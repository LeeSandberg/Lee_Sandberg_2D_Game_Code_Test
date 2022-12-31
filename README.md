# Lee_Sandberg_Sozap_Code_Test
Uses Unity Tilemap and it's preview Tilemap extra https://forum.unity.com/threads/tilemap-extras-preview-package-is-now-available.962664/ 
(Disclaimer: which at the time was the codes largest flaw as it fellt un finnished at the time and I had to make a lot of coding to compensate for that, I would have saved time not using it. Later I changed the renderer in Unity and wanted to add a dynamic spotlight to the player character, where I just added an uggly magic number in the number of child nodes the player character can have. This is a big no, no in production code. This code was intially a work test assignment, with a long laundry list of features they wanted, so some of the code in here is just too complete that featurelist they wanted.)
The parts are tested together with one level, first. 

The managers was added and code has been moved out to the managers. 

My coding is done in passes.

The diffrent passes have very different focus.

* Go through what functions and variables isn't used and remove them.
* Organize the code from one place, to several managers.
* Writing test functions.
* Debugging.
* Good name conventions.
* Scope of variables.
* Completness.
* Pritify.
* Coding standard ( Well some what of one, could decide ).
* Preformance.
* Comments.
* Documentation.
* API Design.
* Software design.

First pass everything is testing, maybe all the code in one file. A lot of code in one function. No real coding style either. A lot of querky code text basically.
Its not at all neat and organized. Its just brain dump, Try and error.

TileMap in Unity, first time. To make things more interesting the project uses the Unity preview of Tilemap extra.

Reason: At time i couldn't find away in Unity to attach a script to sprite.
I later got to know about using TileBase and inherit from that one and add code to tiles.

https://docs.unity3d.com/Manual/Tilemap-ScriptableTiles.html?_ga=2.137354681.847206861.1620301011-893911062.1616429622

But the code took another route when using the Unity Tilemap extra preview.

It adds brushes to the tile map palette. Like drawing Tiles that are GameObjects with Sprite renders on them.
So You can add script to the GameObject.

Great, but Tilemap didn't return back what gameobject is in a certain tile. Unity tilemap can only do that with  sprites not game objects.

So if you look under the boxes (that are gameobjects with sprite renders) there is actually a transparant sprite hiding.
Which we also have to move when the gameobject box moves. The Box moves by getting to be a child och character for a while and moves with the character.
However sprite under the box moves one tile at a time.
