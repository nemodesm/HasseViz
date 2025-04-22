# HasseViz

A visualiser for Hasse Diagrams.

## Usage

Input files can either be structured as

```
1;0;0
0;1;0
1;1;0
```

(which is referred to as "semibinary") or

```
true;false;false
false;true;false
true;true;false
```

(which is referred to as "text"), where a 1 (or "true") on line x and column y
signifying there is a connection from x to y, and a 0 (or "false") signifying there is none.
The example inputs produce a graph where node 3 points to nodes 1 and 2.

After being loaded, the output can be simplified by using the File menu.

## Compiling

The project was created with Jetbrains Rider, and is therefore set up tu be used with it.
Other IDEs can very likely be used, however none have been tested.

## Limitations

There are no curved edges and the visualisation remains fairly simplistic as of now,
however a graph can be exported to TGF (Trivial Graph Format) for use in other
programs if needed.

On Linux and macOS, the window's menu bar is unfortunately not compatible with a global menu
(Linux) or the menu bar (macOS)