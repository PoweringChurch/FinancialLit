# FinancialLit
Project Files for Financial Literatire Pet simulation in FBLA  
[Official Guidelines](https://www.flfbla.org/fbla-event-introduction-to-programming)

# TODO

## Week of 11/3

### Development

Playerstate additions and adjustment
- [x] Re-implement playerstates and statuses throughout codebase

Functionality
- [x] Make Interact module take into account player state and status when determining how to interact with an object

Tackle automatic pivot selection for furniture
- [x] Implement way of detecting pivot automatically when placing furniture
  *  instead will do it manually. it takes less effort than initally thought and automatic is not worth the time to implement
 
Add money & money tracking
- [x] Add currency
- [x] Keep track of spending
- [x] Add UI
  * Keep implementation of ui very barebones and easy to move, all ui handling will be moved into a central module

Player item inventory
- [x] Add item inventory (different from furniture) that keeps tracks of various items, including pet food, shampoo, 
- [x] Add UI
Display pet needs
- [x] Show pet needs visually on UI, likely best implemented as needs on list at the bottom of screen
  * Ultimately chose different method

Pet personality & behavior
- [x] Pet AI, include
  - [x] Goals (should be very barebones, ex: moving, none. Pet should only ever be busy with 1 task at a time)
    * chose to scrap idea, not flexible or practical for current implementation
  - [x] Statuses (Hungry, dirty, etc.)
    * implemented in a different way than anticipated, still functional. might return to this and change how to it works
  - [x] Behaviors (Determine what to do and when)
  - [x] Sickness affected by statuses

### Cleanup

Make inputs work with InputSystem
- [x] Make a centalized input module

Organize files
- [x] Seperate scripts by what they do

Centralize UI
- [x] Move where and how UI information and interaction is stored & handled
  * This is mostly a backend task, not expected to affect anything that the player sees.

## Week of 11/10

### Development

Add sickness
- [x] If stats are too low, add chance every tick to get sick
  - [x] Sickness should slow and make pet cough occasionally

Implement more areas
- [x] Park (way to gain entertainment quicker)
- [ ] Furniture store
- [ ] Vetinarian (cure sickness)
- [ ] Work (earn money)

Models & animation
- [x] Add pet model
  - [ ] Pet variation, likely breeds of species that player can select
    * currently not very high priority, should be easy to implement whenever
- [x] Add pet animation 
- [x] Add proper pet bowl model
  - [x] w/ filled bowl model

Pet AI & improvements
- [x] Add pet pathfinding
- [x] Hide pet when in build mode

Particles
- [x] Add stench particles when pet hygiene is low
- [x] Add particles for certain furnitures
  - [x] Bathing particles
    * removed in place of a different bathing system
  - [x] Eating particles

Pet selection screen
- [x] Add screen that allows for pet selection when entering a new save file
- [ ] Allow for pet renaming
- [x] Add display of pet name ingame, as currently there is no way to actually see it beside notifications

Stylize UI
- [x] Determine color scheme
- [x] Select font
- [x] Stylize buttons

Music and SFX
- [ ] Find suitable sound effects
- [ ] Find suitable music

Settings
- [ ] Add settings such as camera speed, game speed, etc.

Visual Overhaul
- [x] Make style more consistent

Tutorial
- [ ] Add simple game tutorial

Saving & Loading
- [x] Implement saving and loading
  - [x] Player home configuration
  - [x] Pet states and statuses
  - [x] Save into json file?

Furniture additions
- [ ] Add as much furniture as possible with remaining time

### Cleanup

Bugfixing
- [ ] Find and resolve bugs in game
- [ ] Get play testers to review if possible

#### Extra, only do if spare time
Add support for controller
