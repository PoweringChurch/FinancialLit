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
- [ ] Implement way of detecting pivot automatically when placing furniture

Add money & money tracking
- [x] Add currency
- [x] Keep track of spending
- [x] Add UI
  * Keep implementation of ui very barebones and easy to move, all ui handling will be moved into a central module

Player item inventory
- [ ] Add item inventory (different from furniture) that keeps tracks of various items, including pet food, shampoo, 
- [x] Add UI
Display pet needs
- [ ] Show pet needs visually on UI, likely best implemented as needs on list at the bottom of screen

Pet personality
- [ ] Pet AI, include
  - [ ] States (should be very barebones, ex: moving, none. Pet should only ever be busy with 1 task at a time)
  - [ ] Statuses (Hungry, sick, etc.) 
  - [ ] Behaviors (Determine what to do and when)

Saving & Loading
- [ ] Implement saving and loading
  - [ ] Player home configuration
  - [ ] Pet states and statuses
  - [ ] Save into json file?

### Cleanup

Make inputs work with InputSystem
- [ ] Make a centalized input module

Organize files
- [ ] Seperate scripts by what they do

Centralize UI
- [ ] Move where and how UI information and interaction is stored & handled
  * This is mostly a backend task, not expected to affect anything that the player sees.

Stylize UI
- [ ] Determine color scheme
- [ ] Select font
- [ ] Stylize buttons

### Misc
Add support for controller
* Mobile comes later/NEVER, too much ui work
