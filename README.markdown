This is ontogeny simulation project I've inherited from Mikhail Markov.

His only constraint on license was that the project is not be used for commercial purposes.

It is written in C# and is being migrated from Managed DirectX to MonoGame.

![Sample Shot](https://raw.github.com/alamar/EvoDevo3D/master/Shots/welcome.png)

Use "Clone or download -> Download ZIP" on the right over file list.

Run EvoLauncher from `v1.0` directory. Use `gastrula.gp` from Models directory as an example. Should be runnable with [.Net runtime 4.5](https://www.microsoft.com/ru-Ru/download/details.aspx?id=30653) under Windows. Linux will probably need Mono & MonoGame, didn't test on Mac.

Evolution area controls:

* q, e - rotate clockwise-counterclockwise
* w, a, s, d - rotate up-left-down-right
* r, f - bring to back, bring to front (shift - faster, alt - slower)
* W, A, S, D, up, left, down, right - move
* p - screenshot
* Space - pause (be persistent!)
* X - exit
