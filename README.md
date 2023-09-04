These scripts were written for the Unity game engine.

They are either:
	Components, which are attached to entities and determine some data and behaviour,
	or
	ScriptableObjects which just hold persistant data (denoted by SO in the name).

I believe this was one of my first goes at unity, so some of the logic/code style is not good.


Short description of project:

The game is an automation sim silimar to factorio.

- One system where you can place "buildings" on a grid.
- Grid does translations between world and grid space/co-ordinates as well as holds references to placed buildings.

- A system for creation of items using recipes
- Buildings have input/s and output/s

- A system for moving objects between buildings using conveyor belts
