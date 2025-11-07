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
- [ ] Pet AI, include
  - [ ] Goals (should be very barebones, ex: moving, none. Pet should only ever be busy with 1 task at a time)
  - [x] Statuses (Hungry, dirty, etc.)
    * implemented in a different way than anticipated, still functional. might return to this and change how to it works
  - [ ] Behaviors (Determine what to do and when)
  - [x] Sickness affected by statuses

Models
- [ ] Add pet model

Saving & Loading
- [ ] Implement saving and loading
  - [ ] Player home configuration
  - [ ] Pet states and statuses
  - [ ] Save into json file?

Furniture additions
- [ ] Add as much furniture as possible with remaining time

### Cleanup

Make inputs work with InputSystem
- [x] Make a centalized input module

Organize files
- [x] Seperate scripts by what they do

Centralize UI
- [x] Move where and how UI information and interaction is stored & handled
  * This is mostly a backend task, not expected to affect anything that the player sees.

Stylize UI
- [ ] Determine color scheme
- [ ] Select font
- [ ] Stylize buttons

#### Extra
Add support for controller
