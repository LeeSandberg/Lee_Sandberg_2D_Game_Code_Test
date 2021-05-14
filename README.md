# Lee_Sandberg_Sozap_Code_Test

Uses Unity Tilemap and it's preview Tilemap extra https://forum.unity.com/threads/tilemap-extras-preview-package-is-now-available.962664/


The first commit, the state of it is broken after a reorganisation of the code after all four levels where introduced. 

The parts are tested together with one level, first. 

The managers was added and code has been moved out to the managers. 
But You can see my process.

The code is done in passes.
The diffrent passes have very different focus.

* Keep broken code and return to them later, rather than obsess over them.
* Go through what functions and variables isn't used and remove them.
* Organizing out the code from one place, to several managers.
* Writing test functions.
* Debugging.
* Good name convetions.
* Scope of variables.
* Completness.
* Pritify.
* Coding standard ( Well some what of one, could decide ).
* Preformance.
* Comments.
* Documentation.
* API Design.
* Software design.

First pass everything is testing, maybe all the code in one file. Alot of code in one function. No real coding style either. A lot of querky code text basically.
Its not at all neat and organized. Its just brain dump, Try and error.

This one used TileMap in Unity which I never used. To make things more interesting I added the preview of Tilemap extra.

I wanted to use the preview because, at time i couldn't find away in Unity to attach a script to sprite.
I later got to know about using TileBase and inherit from that one and add code to tiles.

https://docs.unity3d.com/Manual/Tilemap-ScriptableTiles.html?_ga=2.137354681.847206861.1620301011-893911062.1616429622

But the code took another rout using the Tilemap extra preview.

It adds brushes to the tile map palette. Like drawing Tiles that are GameObjects witm Sprite renders on them.
So You can add script to the GameObject.

Great, but Tilemap can us back what gameobject tile is in a certain tile.
Tilemap can only do that  with  sprites.

So if you look closly under the boxes (that are gameobjects with sprite renders) there is actuallyu a transparant sprite hiding.
Which we also have to move when the gameobject box moves. The Box moves by getting to be a child och character for a while and moves with the character.
However sprite under the box moves one tile at a time.




