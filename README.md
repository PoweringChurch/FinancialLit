# Intro to Programming Submission Documentation

You are viewing the markdown file version of the docs!

This is an OLD VERSION! Refer to the pdf version of the game docs for the final version of the docs.

Project Files for Financial Literature Pet Simulation in FBLA  
[Official Guidelines](https://www.flfbla.org/fbla-event-introduction-to-programming)

## Overview

### Revision History

Version 1.00, Xavier McCoy, November 18 2025, Initial version

### Table of Contents

1. Introduction
    * Scope & conventions
2. Imports & Other External Resources
3. Target Systems
    * Windows 10
    * Windows 11
    * macOS
4. Development Software
    * Unity
5. Development System
    * Windows 11
    * macOS
6. Specification
    * Concept
    * Pet
    * Finance & resource management
    * Building system
    * Areas and travelling
7. Gameplay
    * Gameplay loop
    * Saving and loading
    * Player input
    * Player interaction with world
8. Interface
    * Main menu
    * Credits & attribution screen
    * Options screen
    * Action radial menu
    * Travel menu
    * Placement menu
    * Pet stats menu
    * Pet status effect display
9. Team
10. Time

---

## 1. Introduction

My Dog Home is a videogame centered around taking care of a pet, featuring pet statistics (stats), a finance system that keeps track of spendings, and a furniture building system allowing for home customization.

### Scope & Conventions

This document provides comprehensive technical and design documentation for My Dog Home, created as a submission for the FBLA Introduction to Programming event. The documentation covers all aspects of the project including system requirements, development tools, game specifications, gameplay mechanics, and user interface design.

**Document Conventions:**
- Technical terms are introduced in context and explained when first used
- Code references and file names are formatted in `monospace`
- Interface elements are described in detail with accompanying screenshots where applicable
- All monetary values referenced in the game are in fictional currency units

---

## 2. Imports & Other External Resources

Below is an exhaustive list of the packages and other external resources used during the development of My Dog Home.

**Unity Packages:**
- Unity UI (built-in) - User interface system
- TextMeshPro (built-in) - Enhanced text rendering
- Unity Input System - Input handling

**External Assets:**
- Yughues Free Wooden Floor Materials - Yughues
- Yughues Free Architectural Materials - Yughues
- Low Poly Simple Furniture FREE - Gobormu
- Bedroom / Interior Low Poly assets - Fries and Seagull
- Bedrooms & Bathroom / Interior Low Poly assets - Fries and Seagull
- 30 Stylized Textures FREE - Billion Mucks
- Seamless Hand-Drawn Textures - Elder Crivelari Teixeira
- 3D Stylized Animated Dogs Kit - Bublisher
- Foliage Pack Free - Jake Sullivan
- Quick Outline - Chris Nolet

- Music - Pixabay
- Icon assets - Freepik, Icons8

**Development Tools:**
- Unity Editor version 6.0
- Visual Studio Code
- Github & Github Desktop

---

## 3. Target Systems

My Dog Home is fully functional on Windows 10, Windows 11, and macOS computers.

## 4. Development Software

The development of My Dog Home was completed using the Unity game development engine, version 6.0. Unity was selected for this project due to its:

The C# programming language was used for all game logic, adhering to FBLA Introduction to Programming event requirements. The project structure follows Unity's standard organization with separate folders for Scripts, Scenes, Prefabs, Materials, and other asset types.

---

## 5. Development System

The development of My Dog Home was completed during school hours and at home using multiple systems to ensure compatibility and functionality across platforms.

### School Development Environment
- **Operating System:** macOS
- **Hardware:** School-provided Mac computer
- **Development Focus:** Minor development work, minor bugfixes, testing, and macOS compatability testing.
- **Hours:** 8:30AM - 10:00AM, first block at LMS

### Home Development Environment
- **Operating System:** Windows 11
- **Hardware:** Personal computer
- **Development Focus:** Primary development work, testing, and Windows compatability testing.
- **Hours:** After-school and weekend development sessions

This development approach ensured that My Dog Home functions properly on both Windows and macOS systems, with regular testing conducted on both environments throughout the development cycle. The compatability for Windows and macOS systems came about as a result of the necessity of working from both home and school on this project.

---

## 6. Specification

### Concept

My Dog Home is a pet simulation game designed to teach financial literacy through gameplay. Players are responsible for maintaining their pet's wellbeing across four core stats while managing a budget and tracking expenditures. The game combines pet care mechanics with resource management to create an experience that emphasizes responsible spending and budget awareness.

**Main Objectives:**
- Maintain pet happiness by managing hunger, hygiene, energy, and entertainment stats
- Track and categorize all spending across multiple expense types
- Balance immediate pet needs with long-term financial planning
- Learn the consequences of neglecting either pet care or financial management

### Pet

The pet system features four core statistics that decay over time and must be actively maintained:

**Statistics:**
- **Hunger:** Decreases at a constant rate, restored by feeding the pet
- **Hygiene:** Decreases over time, restored by bathing the pet
- **Energy:** Decreases slowly over time, restored through resting
- **Entertainment:** Decreases over time, restored by playing or visiting the park

**Status Effects:**
The pet can acquire temporary status effects that modify gameplay:

- **Content:** All four stats above 70%, reduces stat drain by 10%
- **Playful:** Random chance when energy and entertainment exceed 60%, increases entertainment recovery by 10% and movement speed by 5%
- **Loved:** Random chance when hunger and hygiene exceed 60%, reduces stat drain by 5% and increases movement speed by 5%
- **Sick:** Can occur when stats are too low, halves all recovery rates, requires veterinary care to cure
- **Immune:** Granted after veterinary treatment, temporarily prevents sickness
- **Worn Out:** Acquired after extended time at the park, increases sleep energy recovery by 15%

**Pet Behavior:**
The pet autonomously roams when not occupied. Players can issue commands through the radial action menu. The pet's movement speed is influenced by its energy level and active status effects.

### Finance & Resource Management

The finance system tracks all player spending across four categories:

**Spending Categories:**
- **Food:** Purchases of pet food
- **Hygiene:** Purchases of shampoo
- **Furniture:** Purchases of furniture and home items
- **Healthcare:** Veterinary visits

**Resources:**
Players manage three resources:
- **Money:** Primary currency, earned through the work minigame
- **Food:** Consumable items used to fill food bowls
- **Shampoo:** Consumable items used during bathing to restore hygiene

The spending breakdown is accessible by clicking the balance display, promoting awareness of where money has been allocated.

**Earning Money:**
Players earn money by completing work shifts. Work is initiated by interacting with a monitor furniture item and selecting "Go to Work" from the action menu. This launches an order fulfillment minigame where players must:
- Complete 10 customer orders per shift
- Each order consists of 3-5 random items (Ball, Brush, Treat, or Shampoo)
- Orders must be completed within 8 seconds each
- Payment ranges from $10-$20 per order based on completion speed
- Faster completion yields higher payment (time bonus)
- Total shift earnings are displayed at the end and added to the player's balance

### Building System

The building system allows players to customize their home environment with purchased furniture:

**Functionality:**
- Only accessible when the player is in the home area
- Activated through the Build/Placement button in the bottom-right menu
- Displays available furniture items for selection
- Players can place selected furniture within the home space
- Furniture includes functional items (beds for resting, toys for playing, monitors for working) and decorative pieces

**Furniture Types:**
- **Beds:** Allow the pet to sleep and recover energy
- **Toys:** Enable playing to restore entertainment
- **Monitors:** Provide access to the work minigame
- **Food Bowls:** Must be filled with food items before the pet can eat from them, consuming food from resources in the process
- **Decorative Items:** Personalize the home environment

Furniture must be purchased from the Furniture Store before it becomes available for placement.

### Areas and Travelling

The game features five distinct areas accessible through the travel menu:

**Home:**
- Primary play area where the pet lives
- Only location where furniture building is enabled
- Contains all placed furniture and interactive objects

**Park:**
- Outdoor area where pets can play and exercise
- Increases entertainment recovery rate by 5%
- Extended time at the park (40 ticks, 48 seconds) triggers the Worn Out status effect

**SmartyPets:**
- Pet supply store selling consumable items
- Available items: Pet food, pet shampoo, pet beds, pet toys
- Players purchase items through the context action menu on displayed products

**Pet Hospital:**
- Veterinary clinic for treating sick pets
- Cures the Sick status effect
- Grants the Immune status effect, temporarily preventing future sickness
- Healthcare costs are tracked in the Healthcare spending category

**Furniture Store:**
- Retail location for purchasing furniture
- Offers various furniture items for home customization
- Purchases tracked in the Furniture spending category

Players navigate between areas using the travel menu, which displays all available locations as selectable buttons.

---

## 7. Gameplay

### Gameplay Loop

The core gameplay loop revolves around maintaining pet happiness while managing financial resources:

1. **Monitor Pet Stats:** Check the pet's hunger, hygiene, energy, and entertainment levels through the mood display
2. **Identify Needs:** Determine which stats require attention based on current levels and visible status effects
3. **Resource Check:** Verify available food, shampoo, and money supplies
4. **Take Action:** 
   - Fill food bowls with food items, then command the pet to eat from them to restore hunger
   - Bathe the pet with shampoo to restore hygiene
   - Command the pet to sleep on a bed to restore energy
   - Play with toys or visit the park to restore entertainment
5. **Restock Supplies:** When consumables run low, travel to shops to purchase more
6. **Earn Money:** When funds are insufficient, complete work shifts to earn income
7. **Track Spending:** Review spending breakdown to understand financial allocation
8. **Prevent Sickness:** Maintain all stats above critical thresholds to avoid veterinary costs

The loop encourages players to balance immediate pet care needs with resource conservation and financial planning.

### Saving and Loading

The game implements a multiple save file system:

**Saving:**
- Manual save available through the in-game menu
- Saves current pet stats, status effects, player resources, and spending data
- Saves furniture placement and home customization
- "Save and Quit" option saves progress before returning to main menu

**Loading:**
- Save files displayed in order from most recently accessed to least recent
- Clicking a save file loads all associated data
- Game state is fully restored, including pet condition and resource levels

**New Save Creation:**
- New save button (shown as a green + icon on the save screen) prompts for pet name input
- After naming, player enters the game and is offered a tutorial
- New saves start with starter furniture items (Pet Bed, Small Bed, Work Computer, Food Bowl, Bathroom Vanity, Box Bath, Toy Train, Couch, Toilet, Rectangle Table)
- Debug mode option available to start with additional resources for testing

**Save Deletion:**
- Available through in-game menu
- Requires confirmation through yes/no popup to prevent accidental deletion

### Player Input

Player input is primarily mouse-based with context-sensitive interactions.

**Camera Controls:**
- WASD to move camera
- Mouse scroll to zoom
- Camera speeds adjustable in settings

**Interaction:**
- Hover over interactive objects to display their names
- Click objects to open the radial action menu
- Click UI elements to access menus and information

### Player Interaction with World

Players interact with the game world through a context-sensitive action menu system. Clicking on interactive objects opens a radial menu with available actions.

**Notable Interactions:**
- **Work System:** Clicking monitors allows access to "Go to Work," which starts an order fulfillment minigame where players complete customer orders by dragging requested items into a delivery box within time limits
- **Furniture:** Beds, toys, and food bowls provide ways to restore pet stats
- **Shops:** Navigate to shop areas and click products to purchase items
- **Pet Commands:** Click the pet to access movement and behavior commands

Visual effects indicate pet status, such as particles when hygiene is low.

---

## 8. Interface

### Main Menu

The main menu serves as the entry point to My Dog Home:

**Elements:**
- **Game Flag:** Stylized game title prominently displayed at the top
- **Start Button:** Directs players to the save file selection screen
- **Options Button:** Opens the options menu
- **Credits Button:** Opens the credits and attribution screen
- **Quit Button:** Closes the application

The main menu provides clear navigation to all major game functions, ensuring players can quickly access saved games, adjust settings, or view credits.

### Save File Management

The save file menu displays all existing save files in a list format, ordered from most recently accessed to least recently accessed. This organization helps players quickly find their active games.

**Features:**
- **Load Save:** Clicking on any displayed save file loads that game
- **New Save Button:** Creates a new save file, first prompting the player to name their pet
- **Tutorial Prompt:** After creating a new save and naming the pet, players are asked if they want to complete a short tutorial

This system allows for multiple playthroughs and easy save management without confusion.

### In-Game HUD (Heads-Up Display)

The in-game interface is organized into four main screen regions to provide essential information without cluttering the view:

#### Bottom Left - Pet Status
- **Mood Display:** Shows the pet's current emotional state
- **Expandable Stats Panel:** Clicking the mood display reveals statistics:
  - Hunger level
  - Hygiene level
  - Fun level
  - Energy level
- **Status Effect List:** Displays active status effects as icons below the mood display
  - Hovering over icons reveals specifications of each effect

#### Top Left - Financial Information
- **Current Balance:** Displays the player's available money
- **Resource Counts:** Shows quantities of food and shampoo items below the balance
- **Spending Breakdown:** Clicking on the balance reveals expenditures:
  - Hygiene expenses
  - Food expenses
  - Furniture expenses
  - Healthcare expenses

#### Bottom Right - Action Menu
- **Collapsed by Default:** Appears as a small tab with an arrow indicator
- **Expansion:** Clicking the arrow expands the menu to reveal action buttons
- **Available Actions:**
  - Build/Placement button
  - Travel button
- **Collapse Function:** Clicking the arrow again collapses it

#### Top Right - Game Menu Button
Opens the in-game pause menu overlay with the following options:
- **Continue:** Returns to gameplay
- **Options:** Opens the settings screen
- **Save Game:** Saves current progress without exiting
- **Save and Quit:** Saves progress and returns to main menu
- **Quit Without Saving:** Returns to main menu without saving
- **Delete Save:** Removes the current save file (requires confirmation via yes/no popup)

### Options Menu

The options menu provides control over game settings:

**Audio Settings:**
- Music Volume

**Gameplay Settings:**
- Zoom Speed
- Camera Movement Speed

**Graphics Settings:**
- Fullscreen
- VSync
- Anisotropic Filtering
- Realtime Reflections
- Anti-Aliasing
- Texture Quality
- Debug Mode

### Credits & Attribution Screen

The credits screen displays acknowledgments and attributions for all resources, assets, and tools used in the development of My Dog Home.

### Action Radial Menu

The radial action menu appears when players click on interactive objects in the game world.

**Functionality:**
- Displays the object's name in hover text before interaction
- Upon clicking, opens a radial menu showing context-specific actions
- Actions vary based on the selected object type

**Example:**
- Clicking on the pet opens a radial menu with options such as commanding the pet to move to the cursor position

### Travel Menu

The travel menu displays available travel locations.

**Design:**
- Resembles a smartphone screen layout
- Displays buttons for each available travel location

**Functionality:**
- Players select their destination by clicking the corresponding button
- Initiates travel to the selected area

### Placement Menu

The placement menu enables furniture customization within the home area.

**Access:**
- Activated by clicking the Build/Placement button in the bottom-right action menu
- Only functional when the player is inside the home area

**Features:**
- Displays available furniture items for placement
- Allows selection of furniture pieces

---

## 9. Team

My Dog Home was solely developed by Xavier McCoy as a submission for the FBLA Introduction to Programming event.

---

## 10. Time

My Dog Home's development was completed throughout October 2025 through November 2025.

**Total Development Time:** Approximately 2 months