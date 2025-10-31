# FinancialLit
Project Files for Financial Lit pets

## TODO (prio order):

Inventory system
* mostly done, placement needs to reduce count of objects.

Add areas to visit
* also comes with ui; don't expect to be too bad, actually
* Planned: furniture shop, vet, park, grocery store
  * furniture shop is high prio
* with this comes finally deciding what type of pet it is

Buying furniture system
* must its own area
  * make a shop area where player buys furniture?

Room placement
* Rooms should be able to move with all of its furniture in the correct place
  *  this will likely look something like;
  *  ``` room class.furniture class[]
     furniture.gameobj
     furniture.position (relative to room)
     furniture.orientation
* Rooms will only be adjascent to other rooms

Pet improvement
* Have pet randomly do things
  * likely just add goals to existing pet class
* also make pet show needs visually. probably just going to be like thought bubbles hovering from head
  * "I'm hungry!", "I'm bored!", "I feel bad...", "I'm sick!"

Saving game
* and loading game
  * json files in clutch

Pretty up the UI
* also comes with at long last naming the project

Add support for controller
* Mobile comes later/NEVER, too much ui work
