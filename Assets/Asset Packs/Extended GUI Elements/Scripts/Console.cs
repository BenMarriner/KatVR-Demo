using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    public class Console : UIBehaviour, IUpdateSelectedHandler, IPointerDownHandler
    {
        [SerializeField]
        protected Image _cursor;
        [SerializeField]
        protected Text _consoleText;
        [SerializeField]
        private bool _singleton = false;
        [SerializeField]
        private float _blinkRate = 0.85f;
        [SerializeField]
        private bool _useEscapeChar;
        [SerializeField]
        private char _escapeChar = '>';
        [SerializeField]
        private bool _allowInput = false;
        [SerializeField]
        private bool _isPasswordLine = false;
        [SerializeField]
        private bool _usePasswordChar = false;
        [SerializeField]
        private char _passwordChar = '*';
        [SerializeField]
        TouchScreenKeyboard _inputKeyboard;

        protected int _commandIndex = 0;
        protected List<string> _previousCommands = new List<string>();
        protected string _text = "";
        protected float _blinkStartTime;
        protected int _index;
        protected bool _shouldUpdate = false;

        private static Console _instance;
        protected Event _event = new Event();
        protected TextGenerator _textGenerator;

        protected Queue<ReadCallback> _readCallbackQueue = new Queue<ReadCallback>();
        protected Queue<ReadlineCallback> _readlineCallbackQueue = new Queue<ReadlineCallback>();
        protected List<ConsoleCommand> _commands = new List<ConsoleCommand>();

        /// <summary>
        /// Singleton instance of a "main" console, used for easy access to a single console.
        /// </summary>
        public static Console instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Is it allowed to enter inputs into the console. NOTE: If this is true and the console is active ALL input will be picked up!
        /// </summary>
        public bool allowInput
        {
            get
            {
                return _allowInput;
            }
            set
            {
                _allowInput = value;
            }
        }

        /// <summary>
        /// Is the current line a passwordline?
        /// </summary>
        public bool isPasswordLine
        {
            get
            {
                return _isPasswordLine;
            }
            set
            {
                _isPasswordLine = value;
            }
        }

        /// <summary>
        /// Should there be used special chars for password? If false and passwordline is true the line will be empty when entering a password.
        /// </summary>
        public bool usePasswordChar
        {
            get
            {
                return _usePasswordChar;
            }
            set
            {
                _usePasswordChar = value;
            }
        }

        /// <summary>
        /// The char used to display characters when password line and usepasswordchar is true.
        /// </summary>
        public char passwordChar
        {
            get
            {
                return _passwordChar;
            }
            set
            {
                _passwordChar = value;
            }
        }

        /// <summary>
        /// The char which should be displayed in the beginning of each line in the console-
        /// </summary>
        public char escapeChar
        {
            get
            {
                return _escapeChar;
            }
            set
            {
                _escapeChar = value;
            }
        }

        /// <summary>
        /// Should there be a escape char in the console?
        /// </summary>
        public bool useEscapeChar
        {
            get
            {
                return _useEscapeChar;
            }
            set
            {
                _useEscapeChar = value;
            }
        }

        /// <summary>
        /// The blink rate of the cursor
        /// </summary>
        public float blinkRate
        {
            get
            {
                return _blinkRate;
            }
            set
            {
                _blinkRate = value;
            }
        }

        /// <summary>
        /// Gets or Sets the text of the console
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_useEscapeChar)
                {
                    if (!_isPasswordLine)
                    {
                        _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + escapeChar + value;
                        _index = value.Length + 1;
                    }
                    else
                    {
                        if (usePasswordChar)
                        {
                            _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + escapeChar + new string(_passwordChar, _text.Length);
                            _index = _text.Length + 1;
                        }
                    }
                }
                else
                {
                    if (!_isPasswordLine)
                    {
                        _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + value;
                        _index = value.Length;
                    }
                    else
                    {
                        if (usePasswordChar)
                        {
                            _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + new string(_passwordChar, value.Length);
                            _index = value.Length;
                        }
                    }
                }
                _text = value;
            }
        }

        protected override void Awake()
        {
            if (_singleton)
            {
                if (instance == null)
                {
                    instance = this;
                }
                else
                {
                    Debug.LogWarning("Console singleton is already set to another instance and will use " + instance.name);
                }
            }
        }

        protected override void Start()
        {
            _consoleText.text += "\n";
            if (useEscapeChar)
            {
                _consoleText.text += escapeChar;
                _index = 1;
            }
        }

        /// <summary>
        /// Handler for enabling the console.
        /// </summary>
        protected virtual void OnEable()
        {
            _blinkStartTime = Time.unscaledTime;
            _cursor.enabled = true;
        }

        /// <summary>
        /// Handler for disabling the console.
        /// </summary>
        protected override void OnDisable()
        {
            _cursor.enabled = false;
        }

        /// <summary>
        /// Updates the input area, and console component each frame.
        /// </summary>
        protected virtual void LateUpdate()
        {
            //cursor blink
            _cursor.enabled = (Time.unscaledTime - _blinkStartTime) % (1f / blinkRate) < (1f / blinkRate / 2);

            if (!_allowInput)
            {
                return;
            }

            if (_shouldUpdate)
            {
                if (_textGenerator == null)
                {
                    _textGenerator = new TextGenerator();
                }

                UpdateTextgen();
                UpdateSize();
                UpdateCursor();
                _shouldUpdate = false;
            }

            if (_textGenerator == null)
            {
                _shouldUpdate = true;
                return;
            }

            if ((Application.isMobilePlatform || Application.isConsolePlatform) && _inputKeyboard != null)
            {
                if (_inputKeyboard.active && !_inputKeyboard.wasCanceled)
                {
                    _text = String.Copy(_inputKeyboard.text);
                }

                if (_readCallbackQueue.Count != 0 && _text.Length >= 1)
                {
                    _readCallbackQueue.Dequeue().Invoke(this, _text[0]);
                    _inputKeyboard.active = false;
                    _inputKeyboard = null;
                }

                if (!_inputKeyboard.active && _inputKeyboard != null)
                {
                    if (GetInputLine() != string.Empty || GetInputLine() != "")
                    {
                        if (_useEscapeChar)
                        {
                            if (!_isPasswordLine)
                            {
                                _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + escapeChar + _text;
                                _index = _text.Length + 1;
                            }
                            else
                            {
                                if (usePasswordChar)
                                {
                                    _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + escapeChar + new string(_passwordChar, _text.Length);
                                    _index = _text.Length + 1;
                                }
                            }
                        }
                        else
                        {
                            if (!_isPasswordLine)
                            {
                                _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + _text;
                                _index = _text.Length;
                            }
                            else
                            {
                                if (usePasswordChar)
                                {
                                    _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - GetInputLine().Length) + new string(_passwordChar, _text.Length);
                                    _index = _text.Length;
                                }
                            }
                        }
                    }
                    else if (_text != string.Empty)
                    {
                        _consoleText.text += _text;
                    }

                    if (_inputKeyboard.done && !_inputKeyboard.wasCanceled)
                    {
                        if (useEscapeChar)
                        {
                            _consoleText.text += "\n" + escapeChar;
                            _index = 1;
                        }
                        else
                        {
                            _consoleText.text += "\n";
                            _index = 0;
                        }
                        if (_readlineCallbackQueue.Count != 0)
                        {
                            _readlineCallbackQueue.Dequeue().Invoke(this, _text);
                        }
                        else
                        {
                            ProcessCommand(_text);
                        }
                        _text = "";
                    }
                    _inputKeyboard = null;
                }
            }

            while (Event.PopEvent(_event))
            {
                if (_event.type == EventType.KeyDown)
                {
                    _shouldUpdate = true;
                    switch (_event.keyCode)
                    {
                        case KeyCode.Backspace:
                            if ((useEscapeChar && _index > 1) || (!useEscapeChar && _index > 0))
                            {
                                _text = _text.Remove(_index - ((useEscapeChar) ? 2 : 1), 1);
                                if (!isPasswordLine || _usePasswordChar)
                                {
                                    _consoleText.text = _consoleText.text.Remove((_consoleText.text.Length - GetInputLine().Length) + _index - 1, 1);
                                }
                                _index--;
                            }
                            continue;

                        case KeyCode.Delete:
                            if (_index < GetInputLine().Length)
                            {
                                _text = _text.Remove(_index - ((useEscapeChar) ? 1 : 0), 1);
                                if (!isPasswordLine || usePasswordChar)
                                {
                                    _consoleText.text = _consoleText.text.Remove((_consoleText.text.Length - GetInputLine().Length) + _index, 1);
                                }
                            }
                            continue;

                        case KeyCode.Home:
                            if (useEscapeChar)
                            {
                                _index = 1;
                            }
                            else
                            {
                                _index = 0;
                            }
                            continue;

                        case KeyCode.End:
                            _index = GetInputLine().Length;
                            continue;

                        // Copy
                        case KeyCode.C:
                            GUIUtility.systemCopyBuffer = _text;
                            break;

                        // Paste
                        case KeyCode.V:
                            if (useEscapeChar)
                            {
                                _text = _text.Insert(_index - 1, GUIUtility.systemCopyBuffer);
                            }
                            else
                            {
                                _text = _text.Insert(_index, GUIUtility.systemCopyBuffer);
                            }
                            if (!isPasswordLine)
                            {
                                _consoleText.text = _consoleText.text.Insert((_consoleText.text.Length - GetInputLine().Length) + _index, GUIUtility.systemCopyBuffer);
                            }
                            else
                            {
                                if (usePasswordChar)
                                {
                                    _consoleText.text = _consoleText.text.Insert((_consoleText.text.Length - GetInputLine().Length) + _index, new string(passwordChar, GUIUtility.systemCopyBuffer.Length));
                                }
                            }
                            _index += GUIUtility.systemCopyBuffer.Length;
                            break;

                        // Cut
                        case KeyCode.X:
                            GUIUtility.systemCopyBuffer = _text;
                            _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - _text.Length);
                            _text = "";
                            if (useEscapeChar)
                            {
                                _index = 1;
                            }
                            else
                            {
                                _index = 0;
                            }
                            break;

                        case KeyCode.LeftArrow:
                            if ((useEscapeChar && _index > 1) || (!useEscapeChar && _index > 0))
                            {
                                _index--;
                            }
                            continue;

                        case KeyCode.RightArrow:
                            if (_index <= GetInputLine().Length - 1)
                            {
                                _index++;
                            }
                            continue;

                        case KeyCode.UpArrow:
                            if (_previousCommands.Count > 0)
                            {
                                if (_commandIndex > 0)
                                {
                                    _commandIndex--;
                                    Text = _previousCommands[_commandIndex];
                                }
                            }
                            continue;

                        case KeyCode.DownArrow:
                            if (_previousCommands.Count > 0)
                            {
                                if (_commandIndex < _previousCommands.Count - 1)
                                {
                                    _commandIndex++;
                                    Text = _previousCommands[_commandIndex];
                                }
                            }
                            continue;

                        // Submit
                        case KeyCode.Return:
                        case KeyCode.KeypadEnter:
                            if (useEscapeChar)
                            {
                                _consoleText.text += "\n" + escapeChar;
                                _index = 1;
                            }
                            else
                            {
                                _consoleText.text += "\n";
                                _index = 0;
                            }
                            if (_readlineCallbackQueue.Count != 0)
                            {
                                _readlineCallbackQueue.Dequeue().Invoke(this, _text);
                            }
                            else
                            {
                                ProcessCommand(_text);
                            }
                            if (_previousCommands.Count <= _commandIndex || _previousCommands[_commandIndex] != _text)
                            {
                                _previousCommands.Add(_text);
                            }
                            _commandIndex = _previousCommands.Count;
                            _text = "";
                            continue;

                        case KeyCode.Escape:
                            if (_text != "")
                            {
                                _consoleText.text = _consoleText.text.Remove(_consoleText.text.Length - _text.Length);
                                _text = "";
                                if (useEscapeChar)
                                {
                                    _index = 1;
                                }
                                else
                                {
                                    _index = 0;
                                }
                                _commandIndex = _previousCommands.Count;
                            }
                            break;
                    }

                    if (_event.character == 0 || _event.character == '\n' || _event.character == '\r' || _event.character == '\t')
                    {
                        continue;
                    }
                    _blinkStartTime = Time.unscaledTime;

                    if (_readCallbackQueue.Count != 0)
                    {
                        _readCallbackQueue.Dequeue().Invoke(this, _event.character);
                    }
                    else
                    {
                        if (useEscapeChar)
                        {
                            _text = _text.Insert(_index - 1, _event.character.ToString());
                        }
                        else
                        {
                            _text = _text.Insert(_index, _event.character.ToString());
                        }
                        if (!isPasswordLine)
                        {
                            _consoleText.text = _consoleText.text.Insert((_consoleText.text.Length - GetInputLine().Length) + _index, _event.character.ToString());
                        }
                        else
                        {
                            if (usePasswordChar)
                            {
                                _consoleText.text = _consoleText.text.Insert((_consoleText.text.Length - GetInputLine().Length) + _index, passwordChar.ToString());
                            }
                        }
                        _index++;
                    }
                }
            }
        }

        /// <summary>
        /// Get the current inputed line in the console.
        /// </summary>
        /// <returns>The current line in the input area.</returns>
        protected virtual string GetInputLine()
        {
            string[] s = _consoleText.text.Split('\n');
            return s[s.Length - 1];
        }

        /// <summary>
        /// Gets the lastline in the text generator.
        /// </summary>
        /// <returns>The last string in the text generator.</returns>
        protected virtual string GetLastLine()
        {
            int index = _textGenerator.GetLinesArray()[_textGenerator.lineCount - 1].startCharIdx;
            if (index >= _consoleText.text.Length)
            {
                return "";
            }
            return _consoleText.text.Substring(index);
        }

        /// <summary>
        /// Cancels inputs from other components.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (_allowInput)
            {
                eventData.Use();
            }
        }

        /// <summary>
        /// Updates the text generator.
        /// </summary>
        protected virtual void UpdateTextgen()
        {
            _textGenerator.Populate(_consoleText.text, _consoleText.GetGenerationSettings(new Vector2(_consoleText.GetComponent<RectTransform>().rect.size.x, float.MaxValue)));
        }

        /// <summary>
        /// Updates the cursor size and position.
        /// </summary>
        protected virtual void UpdateCursor()
        {
            Vector2 cursorsize = new Vector2(1, _textGenerator.GetLinesArray()[_textGenerator.lineCount - 1].height);
            _cursor.rectTransform.sizeDelta = cursorsize;
            if (isPasswordLine && !usePasswordChar)
            {
                Vector2 pos = _textGenerator.characters[_textGenerator.characters.Count - 1].cursorPos;
                _cursor.transform.localPosition = new Vector3(pos.x, pos.y - (cursorsize.y * 0.5f) + _consoleText.GetComponent<RectTransform>().rect.size.y, 0);
            }
            else
            {
                Vector2 pos = _textGenerator.characters[(_textGenerator.characters.Count - GetInputLine().Length) + _index - 1].cursorPos;
                _cursor.transform.localPosition = new Vector3(pos.x, pos.y - (cursorsize.y * 0.5f) + _consoleText.GetComponent<RectTransform>().rect.size.y, 0);
            }
        }

        /// <summary>
        /// Updates the size of the text area for the console.
        /// </summary>
        protected virtual void UpdateSize()
        {
            float size = 0;

            foreach (UILineInfo l in _textGenerator.lines)
            {
                size += l.height;
            }
            if (this.GetComponent<ScrollRect>() != null)
            {
                this.GetComponent<ScrollRect>().scrollSensitivity = _textGenerator.lines[_textGenerator.lineCount - 1].height;
            }
            _consoleText.rectTransform.sizeDelta = new Vector2(_consoleText.rectTransform.sizeDelta.x, size);
        }

        /// <summary>
        /// Writes an output to the console.
        /// </summary>
        /// <param name="output">The output which should be written.</param>
        public virtual void Write(string output)
        {
            _consoleText.text = _consoleText.text.Insert(_consoleText.text.Length - GetInputLine().Length - 1, output);
            _shouldUpdate = true;
        }

        /// <summary>
        /// Writes an output to the console.
        /// </summary>
        /// <param name="output">The output which should be written.</param>
        /// <param name="color">The color of the written output.</param>
        public virtual void Write(string output, Color color)
        {
            Write(output, (Color32)color);
        }

        /// <summary>
        /// Writes an output to the console.
        /// </summary>
        /// <param name="output">The output which should be written.</param>
        /// <param name="color">The color of the written output.</param>
        public virtual void Write(string output, Color32 color)
        {
            string c = "";
            c += color.r.ToString("X2");
            c += color.g.ToString("X2");
            c += color.b.ToString("X2");
            _consoleText.text = _consoleText.text.Insert(_consoleText.text.Length - GetInputLine().Length - 1, "<color=#" + c + ">" + output + "</color>");
            _shouldUpdate = true;
        }

        /// <summary>
        /// Writes a line to the console.
        /// </summary>
        /// <param name="output">The output which should be written.</param>
        public virtual void WriteLine(string output)
        {
            _consoleText.text = _consoleText.text.Insert(_consoleText.text.Length - GetInputLine().Length - 1, "\n" + output);
            _shouldUpdate = true;
        }

        /// <summary>
        /// Writes a line to the console.
        /// </summary>
        /// <param name="output">The output which should be written.</param>
        /// <param name="color">The color of the written output.</param>
        public virtual void WriteLine(string output, Color color)
        {
            string c = "";
            c += ((Color32)color).r.ToString("X2");
            c += ((Color32)color).g.ToString("X2");
            c += ((Color32)color).b.ToString("X2");
            _consoleText.text = _consoleText.text.Insert(_consoleText.text.Length - GetInputLine().Length - 1, "\n" + "<color=#" + c + ">" + output + "</color>");
            _shouldUpdate = true;
        }

        /// <summary>
        /// Changes the text colors of the text in the console
        /// </summary>
        /// <param name="color">The target color</param>
        public virtual void SetTextColor(Color color)
        {
            _consoleText.color = color;
            _cursor.color = color;
        }

        /// <summary>
        /// Changes the background colors of the background in the console
        /// </summary>
        /// <param name="color">The target color</param>
        public virtual void SetBackgroundColor(Color color)
        {
            this.GetComponent<Image>().color = color;
        }

        /// <summary>
        /// Reads the next input and send it to a callback method.
        /// </summary>
        /// <param name="d">A callback method which is used to handle the input.</param>
        public virtual void Read(ReadCallback d)
        {
            _readCallbackQueue.Enqueue(d);
        }

        /// <summary>
        /// Reads the next line of inputs and send it to a callback method.
        /// </summary>
        /// <param name="d">A callback method which is used to handle the input.</param>
        public virtual void ReadLine(ReadlineCallback d)
        {
            _readlineCallbackQueue.Enqueue(d);
        }

        /// <summary>
        /// Processes a command and checks if it's valid, and then executes it if it is.
        /// </summary>
        /// <param name="cmd">The command to process.</param>
        public virtual void ProcessCommand(string cmd)
        {
            string[] args = cmd.Split(' ');

            if (_commands.Exists(c => c.command.ToLower() == args[0].ToLower() || c.AbstractedCommands.Any(s => s.ToLower() == args[0].ToLower())))
            {
                _commands.Find(c => c.command.ToLower() == args[0].ToLower() || c.AbstractedCommands.Any(s => s.ToLower() == args[0].ToLower())).ExecuteCommand(this, args);
            }
            else
            {
                if (args[0] != "")
                {
                    WriteLine("\"" + args[0] + "\" is not recognized as a valid command...");
                    if (useEscapeChar)
                    {
                        _index = 1;
                    }
                    else
                    {
                        _index = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Clear all text from the console.
        /// </summary>
        public virtual void ClearConsole()
        {
            _consoleText.text = "";
            _consoleText.text += "\n";
            _index = 0;
            if (useEscapeChar)
            {
                _consoleText.text += escapeChar;
                _index = 1;
            }
            _text = "";
        }

        /// <summary>
        /// Clear all text from the console.
        /// </summary>
        /// <param name="args">Not used.</param>
        protected virtual void ClearConsole(string[] args)
        {
            ClearConsole();
        }

        /// <summary>
        /// Adds a command to the console which can then be used if it doesn't already exist.
        /// </summary>
        /// <param name="cmd">A console command object which should be added.</param>
        /// <returns></returns>
        public bool RegisterCommand(ConsoleCommand cmd)
        {
            if (_commands.Exists(c => c.command.ToLower() == cmd.command.ToLower()))
            {
                return false;
            }
            else
            {
                _commands.Add(cmd);
                _commands = (List<ConsoleCommand>)_commands.OrderBy(c => c.command).ToList();
                return true;
            }
        }

        /// <summary>
        /// Removes a command.
        /// </summary>
        /// <param name="cmd">The command which should be removed.</param>
        /// <returns>True, if the command were removed.</returns>
        public bool UnRegisterCommand(ConsoleCommand cmd)
        {
            return _commands.Remove(cmd);
        }

        /// <summary>
        /// Removes a command.
        /// </summary>
        /// <param name="cmd">The command which should be removed.</param>
        /// <returns>True, if the command were removed.</returns>
        public bool UnregisterCommandByName(string cmd)
        {
            return _commands.Remove(_commands.Find(c => c.command.ToLower() == cmd));
        }

        /// <summary>
        /// Prints out all commands in the console.
        /// </summary>
        public void DisplayCommands()
        {
            foreach (ConsoleCommand c in _commands)
            {
                if (c.displayInHelp)
                {
                    WriteLine(c.command + ": " + c.help);
                }
            }
        }

        /// <summary>
        /// Prints out all commands in the console.
        /// </summary>
        /// <param name="arags">Not used.</param>
        protected virtual void DisplayCommands(string[] arags)
        {
            DisplayCommands();
        }

        /// <summary>
        /// Adds standard commands
        /// </summary>
        protected virtual void InitalizeCommands()
        {
            RegisterCommand(new ConsoleCommandInvoke("Help", "Show this help message.", true, new string[] { "?" }, DisplayCommands));
            RegisterCommand(new ConsoleCommandInvoke("Cls", "Clears the console.", true, new string[] { "clear" }, ClearConsole));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Application.isMobilePlatform || Application.isConsolePlatform)
            {
                Vector2 localCursor;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
                {
                    _inputKeyboard = TouchScreenKeyboard.Open(_text, TouchScreenKeyboardType.Default, !isPasswordLine, false, isPasswordLine, false, _text);
                    _inputKeyboard.text = _text;
                }
            }
        }
    }

    /// <summary>
    /// Delegate used for reading a line of inputs.
    /// </summary>
    /// <param name="val">The string value which is inputed.</param>
    public delegate void ReadlineCallback(Console console, string val);

    /// <summary>
    /// Delegate used for reading input.
    /// </summary>
    /// <param name="val">The value of the input pressed.</param>
    public delegate void ReadCallback(Console console, int val);
}