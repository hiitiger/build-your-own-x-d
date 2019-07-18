using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MCompiler.CodeAnalysis.Symbols;
using MCompiler.CodeAnalysis.Syntax;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class ControlFlowGraph
    {
        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockBranch> Branches { get; }

        public ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockBranch> branches)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Branches = branches;
        }

        public sealed class BasicBlock
        {
            public List<BoundStatement> Statements { get; } = new List<BoundStatement>();
            public List<BasicBlockBranch> Incoming { get; } = new List<BasicBlockBranch>();
            public bool IsStart { get; }
            public bool IsEnd { get; }

            public List<BasicBlockBranch> Outgoing = new List<BasicBlockBranch>();

            public BasicBlock()
            {
            }


            public BasicBlock(bool isStart)
            {
                IsStart = isStart;
                IsEnd = !isStart;
            }

            public override string ToString()
            {
                if (IsStart)
                    return "<Start>";
                if (IsEnd)
                    return "<End>";

                using (var stringWriter = new StringWriter())
                {
                    foreach (var statement in Statements)
                        statement.WriteTo(stringWriter);

                    return stringWriter.ToString();
                }
            }
        }

        public sealed class BasicBlockBranch
        {
            public BasicBlockBranch(BasicBlock from, BasicBlock to, BoundExpression condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }

            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public BoundExpression Condition { get; }

            public override string ToString()
            {
                if (Condition == null)
                    return string.Empty;
                return Condition.ToString();
            }
        }

        public sealed class BasicBlockBuilder
        {
            private List<BasicBlock> _blocks = new List<BasicBlock>();
            private List<BoundStatement> _statements = new List<BoundStatement>();
            public List<BasicBlock> Build(BoundBlockStatement blockStatement)
            {
                foreach (var statement in blockStatement.Statements)
                {
                    switch (statement.Kind)
                    {
                        case BoundNodeKind.ExpressionStatement:
                        case BoundNodeKind.VariableDeclarationStatement:
                            //add to current block
                            _statements.Add(statement);
                            break;
                        case BoundNodeKind.GotoStatement:
                        case BoundNodeKind.ConditionalGotoStatement:
                        case BoundNodeKind.ReturnStatement:
                            //end block
                            _statements.Add(statement);
                            StartBlock();
                            break;
                        case BoundNodeKind.LabelStatement:
                            //start block
                            StartBlock();
                            _statements.Add(statement);
                            break;
                        default:
                            throw new Exception($"Unexpected statement: {statement.Kind}");
                    }
                }

                EndBlock();

                return _blocks.ToList();
            }

            private void StartBlock()
            {
                EndBlock();
            }

            private void EndBlock()
            {
                if (_statements.Count > 0)
                {
                    var block = new BasicBlock();
                    block.Statements.AddRange(_statements);
                    _blocks.Add(block);
                    _statements.Clear();
                }
            }
        }

        public sealed class GraphBuilder
        {
            private Dictionary<BoundStatement, BasicBlock> _blockFromStatement = new Dictionary<BoundStatement, BasicBlock>();
            private Dictionary<BoundLabel, BasicBlock> _blockFromLabel = new Dictionary<BoundLabel, BasicBlock>();
            private List<BasicBlockBranch> _branches = new List<BasicBlockBranch>();
            private BasicBlock _start = new BasicBlock(isStart: true);
            private BasicBlock _end = new BasicBlock(isStart: false);
            public ControlFlowGraph Build(List<BasicBlock> blocks)
            {
                if (!blocks.Any())
                {
                    Connect(_start, _end);
                }
                else
                {
                    Connect(_start, blocks.First());
                }

                foreach (var block in blocks)
                {
                    foreach (var statement in block.Statements)
                    {
                        _blockFromStatement.Add(statement, block);
                        if (statement is BoundLabelStatement l)
                            _blockFromLabel.Add(l.Symbol, block);
                    }
                }


                for (int i = 0; i < blocks.Count; i++)
                {
                    BasicBlock block = blocks[i];
                    var next = i == blocks.Count - 1 ? _end : blocks[i + 1];

                    foreach (var statement in block.Statements)
                    {
                        var islast = statement == block.Statements.Last();
                        Walk(statement, block, next, islast);
                    }
                }

                var removedBlocks = blocks.Where(x => x.Incoming.Count == 0).ToList();
                blocks = blocks.Where(x => x.Incoming.Count > 0).ToList();
                foreach (var block in removedBlocks)
                {
                    foreach (var branch in block.Outgoing)
                    {
                        branch.To.Incoming.Remove(branch);
                        _branches.Remove(branch);
                    }
                }

                blocks.Insert(0, _start);
                blocks.Add(_end);
                return new ControlFlowGraph(_start, _end, blocks, _branches);
            }

            private void Walk(BoundStatement statement, BasicBlock current, BasicBlock next, bool islast)
            {
                switch (statement.Kind)
                {
                    case BoundNodeKind.LabelStatement:
                    case BoundNodeKind.ExpressionStatement:
                    case BoundNodeKind.VariableDeclarationStatement:
                        if (islast)
                            Connect(current, next);
                        break;
                    case BoundNodeKind.GotoStatement:
                        var gotoStatement = (BoundGotoStatement)statement;
                        var toBlock = _blockFromLabel[gotoStatement.Label];
                        Connect(current, toBlock);
                        break;
                    case BoundNodeKind.ConditionalGotoStatement:
                        var cgs = (BoundConditionalGotoStatement)statement;
                        var thenBlock = _blockFromLabel[cgs.Label];
                        var elseBlock = next;
                        var negCondition = Negate(cgs.Condition);
                        var thenCondition = cgs.JumpIfFalse ? negCondition : cgs.Condition;
                        var elseCondition = cgs.JumpIfFalse ? cgs.Condition : negCondition;
                        Connect(current, thenBlock, thenCondition);
                        Connect(current, elseBlock, elseCondition);
                        break;
                    case BoundNodeKind.ReturnStatement:
                        Connect(current, _end);
                        break;
                    default:
                        throw new Exception($"Unexpected statement: {statement.Kind}");
                }
            }

            private BoundExpression Negate(BoundExpression condition)
            {
                if (condition is BoundLiteralExpression l)
                {
                    var value = (bool)l.Value;
                    return new BoundLiteralExpression(!value);
                }
                var op = BoundUnaryOperator.Bind(SyntaxKind.BangToken, TypeSymbol.Bool);
                return new BoundUnaryExpression(op, condition);
            }

            private void Connect(BasicBlock from, BasicBlock to, BoundExpression condition = null)
            {
                if (condition is BoundLiteralExpression l)
                {
                    var value = (bool)l.Value;
                    if (value)
                        condition = null;
                    else
                        return;
                }
                var branch = new BasicBlockBranch(from, to, condition);
                from.Outgoing.Add(branch);
                to.Incoming.Add(branch);
                _branches.Add(branch);
            }
        }
        public void WriteTo(TextWriter writer)
        {
            string Quote(string text) => "\"" + text.Replace("\"", "\\\"") + "\"";

            writer.WriteLine("digraph G {");

            var blockIds = new Dictionary<BasicBlock, string>();
            for (int i = 0; i < Blocks.Count; i++)
            {
                var id = $"N{i}";
                blockIds.Add(Blocks[i], id);
            }

            foreach (var block in Blocks)
            {
                var id = blockIds[block];
                var label = Quote(block.ToString().Replace(Environment.NewLine, "\\n"));
                writer.WriteLine($"    {id} [label = {label} shape = box]");
            }

            foreach (var branch in Branches)
            {
                var fromId = blockIds[branch.From];
                var toId = blockIds[branch.To];

                var label = Quote(branch.ToString());
                writer.WriteLine($"    {fromId} ->{toId} [label = {label}]");
            }

            writer.WriteLine("}");
        }

        public static ControlFlowGraph Create(BoundBlockStatement body)
        {
            var basicBlockBuiler = new BasicBlockBuilder();
            var blocks = basicBlockBuiler.Build(body);

            var graphBuilder = new GraphBuilder();
            return graphBuilder.Build(blocks);
        }

        public static bool AllPathsReturn(BoundBlockStatement body)
        {
            var graph = Create(body);
            foreach (var branch in graph.End.Incoming)
            {
                var s = branch.From.Statements.Last();
                if (s.Kind != BoundNodeKind.ReturnStatement)
                        return false;
            }
            return true;
        }
    }
}