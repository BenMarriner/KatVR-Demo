Known Issues:

Console: input on mobile devices is now supported but not probably tested completely.
Window: When scaled/Rotated the pointer is slightly off when resizing. - Scale will/might always be "lossy".

- if you find any problems/bugs please report them with clear directions of how to reproduce it.


Components:

Hold Button: A button which react to being held down
Selection Panel: A panel with selection options which includes bool, int, and string
Window: A fully working, dragable and resizable window with multiple events
TwoStagedSelectable: A two staged selectable component with individual active/inactive colors/sprites
Toggle Button: A two staged button
Console: 
FPS Indicator: FPS label with update timer.


Coming Soon:

Tabmenu: A tabmenu with multiple dynamic tabs
Calender
Timers/Clocks


Windows best practises:

The windows are designed to be used in any canvas, however when using multiple windows which is dragable and resizable I would recommend to make a parent object which is streched to the canvas (see window scene for example)

Selection Panel best practises:

Selection panels works well standalone, but putting them into a parent object with a layout group component seems to make them work the best (see demo scene for example)