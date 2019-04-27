using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace MCompiler
{
    internal abstract class Repl
    {
        private List<string> _submissionHistory = new List<string>();
        private int _submissionHistoryIndex = 0;
        private bool _done;

        public void Run()
        {
            while (true)
            {
                var text = EditSubmission();
                if (string.IsNullOrEmpty(text))
                    return;

                if (text.StartsWith("#"))
                    EvaluateMetaCommand(text);
                else
                {
                    EvaluateSubmission(text);
                    _submissionHistory.Add(text);
                    _submissionHistoryIndex = 0;
                }
            }
        }

        private sealed class SubmissionView
        {
            private readonly Action<string> _lineRenderer;
            private readonly ObservableCollection<string> _submissionDocument;
            private readonly int _cursorTop;
            private int _rendererLineCount;
            private int _currentLineIndex;
            private int _currentCharacter;

            public int CurrentLineIndex
            {
                get => _currentLineIndex;
                set
                {
                    if (_currentLineIndex != value)
                    {
                        _currentLineIndex = value;

                        var line = _submissionDocument[CurrentLineIndex];
                        if (_currentCharacter > line.Length)
                            _currentCharacter = line.Length;
                        UpdateCursorPosition();
                    }
                }
            }
            public int CurrentCharacter
            {
                get => _currentCharacter;
                set
                {
                    if (_currentCharacter != value)
                    {
                        _currentCharacter = value;
                        UpdateCursorPosition();
                    }
                }
            }
            public SubmissionView(Action<string> lineRenderer, ObservableCollection<string> submissionDocument)
            {
                _lineRenderer = lineRenderer;
                _submissionDocument = submissionDocument;
                _submissionDocument.CollectionChanged += submissionDocumentChanged;
                _cursorTop = Console.CursorTop;
                Render();
            }

            private void submissionDocumentChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                Render();
            }

            private void Render()
            {
                Console.CursorVisible = false;

                var lineCount = 0;
                foreach (var line in _submissionDocument)
                {
                    Console.SetCursorPosition(0, _cursorTop + lineCount);
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (lineCount == 0)
                        Console.Write("» ");
                    else
                        Console.Write("· ");

                    Console.ResetColor();
                    _lineRenderer(line);
                    Console.WriteLine(new string(' ', Console.WindowWidth - line.Length - 2));
                    lineCount += 1;
                }

                var numbeOfBlankLines = _rendererLineCount - lineCount;
                if (numbeOfBlankLines > 0)
                {
                    var blackLine = new String(' ', Console.WindowWidth);
                    for (var i = 0; i < numbeOfBlankLines; ++i)
                    {
                        Console.SetCursorPosition(0, _cursorTop + lineCount + i);
                        Console.WriteLine(blackLine);
                    }
                }

                _rendererLineCount = lineCount;
                Console.CursorVisible = true;
                UpdateCursorPosition();
            }

            private void UpdateCursorPosition()
            {
                Console.CursorTop = _cursorTop + _currentLineIndex;
                Console.CursorLeft = 2 + _currentCharacter;
            }
        }

        private string EditSubmission()
        {
            _done = false;
            var document = new ObservableCollection<string>() { "" };
            var view = new SubmissionView(this.RenderLine, document);
            while (!_done)
            {
                var key = Console.ReadKey(true);
                HandleKey(key, document, view);
            }

            Console.WriteLine();
            return string.Join(Environment.NewLine, document);
        }

        private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
        {
            if (key.Modifiers == default(ConsoleModifiers))
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.Escape:
                        HandleEscape(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document, view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(document, view);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                    default:
                        break;
                }
            }
            else if (key.Modifiers == ConsoleModifiers.Control)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }
            }

            if (key.KeyChar >= ' ')
                HandleTyping(document, view, key.KeyChar.ToString());
        }

        private void UpdateDocumentFromHistory(ObservableCollection<string> document, SubmissionView view)
        {
            document.Clear();
            var historyItem = _submissionHistory[_submissionHistoryIndex];
            var lines = historyItem.Split(Environment.NewLine);
            foreach (var line in lines)
                document.Add(line);

            view.CurrentLineIndex = document.Count - 1;
            view.CurrentCharacter = document[view.CurrentLineIndex].Length;
        }

        private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
        {
            _submissionHistoryIndex -= 1;
            if (_submissionHistoryIndex < 0)
                _submissionHistoryIndex = _submissionHistory.Count - 1;
            UpdateDocumentFromHistory(document, view);
        }

        private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
        {
            _submissionHistoryIndex += 1;
            if (_submissionHistoryIndex > _submissionHistory.Count - 1)
                _submissionHistoryIndex = 0;
            UpdateDocumentFromHistory(document, view);
        }

        private void HandleEscape(ObservableCollection<string> document, SubmissionView view)
        {
            document[view.CurrentLineIndex] = string.Empty;
            view.CurrentCharacter = 0;
        }

        private void HandleTab(ObservableCollection<string> document, SubmissionView view)
        {
            const int TabWidth = 4;
            var start = view.CurrentCharacter;
            var spaces = TabWidth - start % TabWidth;
            var line = document[view.CurrentLineIndex];
            line = line.Insert(start, new string(' ', spaces));
            document[view.CurrentLineIndex] = line;
            view.CurrentCharacter += spaces;
        }

        private void HandleEnd(ObservableCollection<string> document, SubmissionView view)
        {
            var line = document[view.CurrentLineIndex];
            view.CurrentCharacter = line.Length;
        }

        private void HandleHome(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentCharacter = 0;
        }

        private void HandleDelete(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLineIndex;
            var line = document[lineIndex];
            var start = view.CurrentCharacter;
            if (start > line.Length - 1)
                return;

            var newLine = line.Substring(0, start) + line.Substring(start + 1);
            document[lineIndex] = newLine;
        }

        private void HandleBackspace(ObservableCollection<string> document, SubmissionView view)
        {
            var lineIndex = view.CurrentLineIndex;
            var line = document[lineIndex];
            var start = view.CurrentCharacter;
            if (start > line.Length)
                return;

            if (start == 0)
            {
                if (lineIndex == 0)
                    return;
                else
                {
                    view.CurrentLineIndex -= 1;
                    line = document[lineIndex - 1];
                    view.CurrentCharacter = document[view.CurrentLineIndex].Length;
                    start = view.CurrentCharacter;
                }
            }
            var newLine = line.Substring(0, start - 1) + line.Substring(start);
            document[lineIndex] = newLine;
            view.CurrentCharacter -= 1;
        }

        private void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLineIndex < document.Count - 1)
                view.CurrentLineIndex += 1;
        }

        private void HandleUpArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLineIndex > 0)
                view.CurrentLineIndex -= 1;
        }

        private void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
        {
            var line = document[view.CurrentLineIndex];
            if (view.CurrentCharacter < line.Length)
                view.CurrentCharacter += 1;
        }

        private void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentCharacter > 0)
                view.CurrentCharacter -= 1;
        }

        private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
        {
            var documentText = string.Join(Environment.NewLine, document);
            if (documentText.StartsWith("#") || IsCompleteSubmission(documentText))
            {
                _done = true;
                return;
            }

            InsertLine(document, view);
        }


        private void HandleControlEnter(ObservableCollection<string> document, SubmissionView view)
        {
            InsertLine(document, view);
            _done = true;
        }
        private static void InsertLine(ObservableCollection<string> document, SubmissionView view)
        {
            var remainder = document[view.CurrentLineIndex].Substring(view.CurrentCharacter);
            document[view.CurrentLineIndex] = document[view.CurrentLineIndex].Substring(0, view.CurrentCharacter);

            if (view.CurrentLineIndex < document.Count - 1)
                document.Insert(view.CurrentLineIndex + 1, remainder);
            else
                document.Add(remainder);
            view.CurrentCharacter = 0;
            view.CurrentLineIndex += 1;
        }

        private void HandleTyping(ObservableCollection<string> document, SubmissionView view, string text)
        {
            var lineIndex = view.CurrentLineIndex;
            var start = view.CurrentCharacter;
            document[lineIndex] = document[lineIndex].Insert(start, text);
            view.CurrentCharacter += text.Length;
        }

        private string EditSubmissionOld()
        {
            StringBuilder textBuilder = new StringBuilder();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (textBuilder.Length == 0)
                    Console.Write("→ ");
                else
                    Console.Write("· ");
                Console.ResetColor();

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        return null;
                    }
                    else if (input.StartsWith("#"))
                    {
                        EvaluateMetaCommand(input);
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var source = textBuilder.ToString();
                if (!IsCompleteSubmission(source))
                    continue;

                return source;
            }
        }

        protected void ClearHistory()
        {
            _submissionHistory.Clear();
        }

        protected virtual void RenderLine(string text)
        {
            Console.Write(text);
        }

        protected abstract bool IsCompleteSubmission(string text);

        protected virtual void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command {input}");
                    Console.ResetColor();
                    break;
            }
        }

        protected abstract void EvaluateSubmission(string source);
    }
}