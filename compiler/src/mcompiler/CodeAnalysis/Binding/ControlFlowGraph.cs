using System;
using System.Collections.Generic;
using System.IO;

namespace MCompiler.CodeAnalysis.Binding
{
    internal sealed class ControlFlowGraph
    {
        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockEdge> Edges { get; }

        public ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockEdge> edges)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Edges = edges;
        }

        public sealed class BasicBlock
        {
            public List<BoundStatement> Statements { get; } = new List<BoundStatement>();
            public List<BasicBlockEdge> Incoming { get; } = new List<BasicBlockEdge>();
            public List<BasicBlockEdge> Outgoing = new List<BasicBlockEdge>();
        }

        public sealed class BasicBlockEdge
        {
            public BasicBlockEdge(BasicBlock from, BasicBlock to, BoundExpression condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }

            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public BoundExpression Condition { get; }
        }

        public void WriteTo(TextWriter writer)
        {
            writer.WriteLine("diagraph G {");

            var blockIds = new Dictionary<BasicBlock, string>();
            for (int i = 0; i < Blocks.Count; i++)
            {
                var id = $"N{i}";
                blockIds.Add(Blocks[i], id);
            }

            foreach(var block in Blocks)
            {
                var id = blockIds[block];
                var label = block.ToString().Replace(Environment.NewLine, "\\1");
                writer.WriteLine($"    {id} [label = {label} shape = box]");
            }

            foreach(var edge in Edges)
            {
                var fromId = blockIds[edge.From];
                var toId = blockIds[edge.To];

                var label = edge.Condition == null ? string.Empty : edge.Condition.ToString();
                writer.WriteLine($"    {fromId} ->{toId} [label = {label}]");
            }

            writer.WriteLine("}");
        }

    }
}