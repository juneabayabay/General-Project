# Mary in the Secret Garden

## Specification-Driven Game Development Documentation

**Project Type:** 2D narrative platformer  
**Target Platform:** PC (Windows initially)  
**Game Engine:** Unity 2D  
**Programming Language:** C#  
**Development Environment:** Unity Editor and Cursor AI  
**Development Methodology:** Specification-Driven Development (SDD)  
**Project Status:** Requirements Analysis and Specification Review  
**Document Version:** 1.0

---

# 1. Instructions for Cursor Agent

You are assisting with the development of *Mary in the Secret Garden*, a student-level 2D narrative platformer developed using Unity and C#.

This document is the authoritative source of truth for the project. All implementation decisions, game mechanics, code changes, tests, and documentation must remain consistent with the approved specification.

## 1.1 Mandatory Development Rules

1. Inspect the existing project before creating, deleting, renaming, or modifying any files.
2. Read this entire README before beginning work.
3. Identify the existing Unity version, project structure, scenes, scripts, packages, assets, and input configuration.
4. Preserve existing working features and project files.
5. Follow the development phases in the order defined in this document.
6. Do not skip requirements analysis, design, planning, implementation, or testing.
7. Do not generate the entire game in one operation.
8. Implement one manageable feature at a time.
9. Write modular, readable, maintainable C# code appropriate for a student project.
10. Use real implementations for required features. Do not substitute fake functionality for working systems.
11. Never claim a feature works unless it has been tested and the result is supported by evidence.
12. Never mark an unexecuted test as passed.
13. Do not introduce combat, weapons, multiplayer, or unrelated gameplay systems.
14. Do not silently change approved mechanics, acceptance criteria, or story elements.
15. Document contradictions, missing requirements, technical limitations, and proposed changes.
16. Ask for approval when a decision materially changes the game design, architecture, scope, or player experience.
17. Maintain a traceability link between every functional requirement, implementation task, and test.
18. Do not overwrite user-created assets or project files without authorization.
19. Do not install packages, change project settings, or perform destructive operations without an appropriate reason and approval.
20. Keep progress reports accurate and distinguish planned, implemented, and verified functionality.

## 1.2 Approval Gates

Cursor must stop at each approval gate.

| Gate | Required output | Permission to proceed |
| --- | --- | --- |
| G1 | Project inspection and complete requirements specification | User approval |
| G2 | Technical design and implementation plan | User approval |
| G3 | Core movement implementation and test results | User approval |
| G4 | Hazards, Resolve, NPC detection, and checkpoint verification | User approval |
| G5 | Collectibles, clues, inventory, and objective verification | User approval |
| G6 | Garden restoration and progression verification | User approval |
| G7 | UI, menus, ending, and optional saving verification | User approval |
| G8 | Final regression testing and documentation | User approval |

Approval of a phase does not automatically approve unrelated changes or the next major phase.

If a dependency fails, stop dependent implementation until the problem is diagnosed and resolved.

---

# 2. Project Overview

## 2.1 Game Concept

Mary Lennox is a lonely young girl who moves into her uncle's mysterious manor after losing her parents.

While exploring the grounds, she discovers a hidden garden that has been neglected for years. With the help of clues, a curious robin, and discoveries throughout the manor, Mary gradually uncovers the garden's secrets.

The player guides Mary through platforming challenges, environmental hazards, exploration, collectible items, locked entrances, and restoration objectives.

As the player progresses, the garden changes from a lifeless and forgotten place into a vibrant environment filled with plants, flowers, and renewed life.

The game emphasizes:

- Discovery and exploration.
- Hope and emotional recovery.
- Friendship and guidance.
- Nature and environmental restoration.
- Curiosity and problem-solving.
- Confidence and personal growth.

There is no combat, no weapon system, and no permanent game-over state.

## 2.2 Problem Statement

The game aims to provide a manageable 2D platforming experience that combines narrative exploration with environmental restoration.

The main development challenge is integrating movement, collision, hazards, collectibles, progression, checkpoints, and visual changes into one coherent experience while maintaining understandable objectives and reliable state management.

The project must remain achievable within a student game development scope.

## 2.3 Project Objectives

The game must allow the player to:

1. Explore the manor grounds.
2. Learn the keyboard controls through gameplay.
3. Follow the robin toward the hidden entrance.
4. Discover clues and collect the buried key.
5. Unlock and enter the secret garden.
6. Collect seeds and flowers.
7. Navigate environmental hazards and avoid being spotted.
8. Recover from mistakes through the Resolve and checkpoint systems.
9. Complete restoration objectives.
10. Observe persistent visual changes in restored areas.
11. Unlock the final blooming garden.
12. Reach an ending that communicates hope, friendship, care, and personal growth.

## 2.4 Target Platform and Player Mode

- **Engine:** Unity 2D.
- **Language:** C#.
- **Target:** Windows PC initially.
- **Player mode:** Single-player.
- **Input:** Keyboard.
- **Presentation:** Two-dimensional side-view platforming.
- **Multiplayer:** Out of scope.
- **Combat:** Out of scope.

The exact Unity version, rendering pipeline, resolution, input package, and build configuration must be determined by inspecting the existing project.

---

# 3. Specification-Driven Development Workflow

The project will use the following development lifecycle.

1. **Requirements analysis:** Inspect the project and establish the approved game specification.
2. **Technical design:** Define the architecture, scene structure, component responsibilities, and state management.
3. **Implementation planning:** Break the work into small tasks with dependencies and test cases.
4. **Incremental implementation:** Build one feature at a time.
5. **Verification:** Run automated tests where appropriate and perform manual gameplay tests.
6. **Regression testing:** Confirm that previously working features remain functional.
7. **Documentation:** Record implementation details, actual results, known issues, and evidence.

## 3.1 Requirement Status Definitions

Every requirement must have one of these statuses:

- **PROPOSED:** Defined in the specification but awaiting approval.
- **APPROVED:** Accepted by the user.
- **PLANNED:** Assigned to an implementation task.
- **IMPLEMENTED:** Code or assets have been created.
- **VERIFIED:** Acceptance criteria have been tested successfully.
- **BLOCKED:** Implementation or verification cannot proceed.
- **FAILED:** A test or acceptance criterion did not pass.
- **DEFERRED:** Explicitly postponed with a documented reason.
- **OUT OF SCOPE:** Not included in the approved project.

Writing code does not automatically change a requirement to VERIFIED.

## 3.2 Change Control

When a requirement needs to change:

1. Identify its requirement ID.
2. Explain the issue or reason for the change.
3. Propose the revised behavior.
4. Identify affected scripts, scenes, requirements, and tests.
5. Obtain user approval.
6. Update the specification and traceability matrix.
7. Implement and test the approved change.

The README remains authoritative until the user approves a revised specification.

---

# 4. Initial Project Inspection

## 4.1 Required Inspection

Before implementation, Cursor must inspect:

- Unity project root.
- Unity version and editor configuration.
- `Assets/` structure.
- `Packages/manifest.json` and relevant package configuration.
- `ProjectSettings/`.
- Existing scenes.
- Existing scripts.
- Prefabs and ScriptableObjects.
- Sprites, textures, audio, fonts, and tile assets.
- Input settings and Input System configuration.
- Existing tests and assembly definitions.
- Build settings or scene inclusion configuration.
- Existing documentation and version-control status, if available.

## 4.2 Inspection Restrictions

During the first phase:

- Do not create, modify, rename, or delete project files.
- Do not generate game scripts.
- Do not create scenes or prefabs.
- Do not import assets.
- Do not install packages.
- Do not change Unity settings.

If project access is unavailable, report the missing access and request the required information. Never claim to have inspected files that were not accessible.

## 4.3 Required Inspection Report

The report must include:

1. Project type and detected Unity version.
2. Existing directory structure.
3. Existing scenes and gameplay systems.
4. Relevant packages and input configuration.
5. Assets that can be reused.
6. Existing features that must be preserved.
7. Missing components and dependencies.
8. Technical risks and unresolved decisions.
9. Recommended architecture.
10. Confirmation that no project files were modified.

**Phase 1 must end with the complete specification presented for approval.**

---

# 5. Gameplay Design

## 5.1 Core Gameplay Loop

The intended gameplay loop is:

1. Explore the environment.
2. Discover clues, objects, and points of interest.
3. Navigate platforms and environmental hazards.
4. Avoid detection by designated NPCs.
5. Collect required items.
6. Complete objectives.
7. Unlock new paths and areas.
8. Restore designated garden locations.
9. Observe changes in the environment.
10. Advance toward the final blooming garden.

Failure to avoid a hazard or NPC causes a temporary setback, not permanent defeat.

## 5.2 Keyboard Controls

| Input | Action |
| --- | --- |
| A | Move left |
| D | Move right |
| Spacebar | Jump |
| E | Interact with designated objects |
| Escape | Pause or resume gameplay |

Controls must be implemented consistently throughout the game.

Interaction and pause functionality must not be assumed to exist until implemented and tested.

If the project uses Unity's Input System package, Cursor must inspect its configuration before selecting an input implementation. Do not mix incompatible input approaches without justification.

---

# 6. Story and Level Progression

## 6.1 Introduction: The Lonely Girl

Mary arrives at her uncle's manor after losing her parents.

The environment is quiet and unfamiliar. An introductory sequence establishes her loneliness and introduces the player to movement, jumping, and exploration.

Required outcomes:

- Introduce Mary.
- Establish the manor setting.
- Display basic controls.
- Allow the player to begin moving and exploring.

## 6.2 Chapter 1: Exploring the Manor Grounds

Mary explores paths, gardens, and outdoor areas.

Gameplay introduces:

- Left and right movement.
- Jumping over small obstacles.
- Brambles and loose stones.
- Environmental exploration.
- Clue discovery.
- A designated checkpoint.

## 6.3 Chapter 2: The Curious Robin

A robin attracts Mary's attention and guides her toward an ivy-covered wall.

The robin must act as a visual guide rather than a combatant or required collectible.

Required outcomes:

- The robin is visually distinguishable.
- Its movement encourages the player to follow it.
- The player can navigate the associated platforming section.
- The player discovers evidence of a hidden entrance.

## 6.4 Chapter 3: The Buried Key

Mary investigates the ivy-covered wall and discovers a buried key.

Required outcomes:

- The key can be collected once.
- The inventory or objective display updates.
- The hidden door remains locked without the required key.
- Interacting with the door after obtaining the key unlocks the entrance.

## 6.5 Chapter 4: The Forgotten Secret Garden

Mary enters a neglected garden.

The initial environment contains muted colors, dead plants, overgrown paths, tangled brambles, and abandoned spaces.

Required outcomes:

- The garden has a distinct visual identity.
- Exploration and clue discovery remain important.
- Seeds and flowers become available.
- More complex platforming is introduced.
- Supporting NPC encounters are incorporated where appropriate.

## 6.6 Chapter 5: Restoring Life

Mary begins restoring the garden by completing objectives.

Possible objectives include:

- Collecting seeds.
- Finding flowers.
- Reaching designated areas.
- Unlocking paths.
- Activating restoration points.

Each completed objective must produce understandable feedback and an appropriate change in game state.

## 6.7 Chapter 6: The Blooming Garden

The final area becomes available after all required restoration objectives are complete.

The environment becomes vibrant and welcoming.

Mary reflects on the hope, friendship, care, and confidence she has developed.

The ending must be reachable through normal gameplay without combat.

---

# 7. Character and Environment Requirements

## 7.1 Mary Lennox

Mary is the playable character.

Required capabilities:

- Idle animation.
- Walking or running animation.
- Jump animation.
- Falling animation.
- Landing behavior.
- Ground and platform detection.
- Collision with solid level geometry.
- Responsive keyboard controls.

Temporary sprites are acceptable during development. They must be clearly identified as temporary and replaceable.

Use original, licensed, or appropriately permitted artwork.

## 7.2 The Robin

The robin must:

- Appear in the manor grounds.
- Move or animate to attract attention.
- Guide the player toward the hidden entrance.
- Avoid unnecessarily blocking Mary.
- Support narrative progression without becoming a combat or collectible character.

## 7.3 Supporting NPCs

The groundskeeper and Mrs. Medlock create simple stealth or avoidance challenges.

Their behavior must be predictable and fair.

Each relevant NPC requires:

- A defined patrol behavior.
- A configurable detection range or area.
- A condition for detecting Mary.
- A condition for ending detection.
- A clear consequence when Mary is spotted.
- A safe recovery path that does not cause a softlock.

Detection should not repeatedly drain Resolve every frame.

## 7.4 Required Environments

1. Manor exterior and grounds.
2. Paths with brambles and loose stones.
3. Ivy-covered wall and hidden entrance.
4. Forgotten secret garden.
5. Partially restored garden.
6. Fully blooming garden.

Use Unity Tilemaps or an equivalent level-building system appropriate to the existing project.

---

# 8. Functional Requirements

The following IDs establish the initial requirements baseline. All requirements remain PROPOSED until the user approves the specification.

## 8.1 Project and Input

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-001 | Project compatibility | The implementation uses the inspected and approved Unity project configuration. |
| FR-002 | Keyboard movement | A moves Mary left and D moves her right. |
| FR-003 | Jumping | Spacebar initiates a jump when the approved jump conditions are satisfied. |
| FR-004 | Ground detection | Mary can distinguish grounded and airborne states. |
| FR-005 | Collision handling | Mary cannot pass through designated solid geometry under normal movement. |
| FR-006 | Animation states | Idle, movement, jumping, falling, and landing behavior are represented appropriately. |
| FR-007 | Interaction | E interacts with eligible objects within a defined interaction range. |
| FR-008 | Pause | Escape pauses gameplay and allows the player to resume. |

## 8.2 Hazards and NPCs

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-009 | Hazard collision | Brambles and designated loose stones have defined collision or trigger areas. |
| FR-010 | Hazard damage | Hazard contact reduces Resolve according to the configured damage rule. |
| FR-011 | Damage protection | Continuous contact with one hazard does not apply damage every frame. |
| FR-012 | NPC patrol | The groundskeeper and Mrs. Medlock follow their configured patrol behavior. |
| FR-013 | NPC detection | NPCs detect Mary within their configured range or detection area. |
| FR-014 | Detection termination | Detection ends when its defined exit conditions are met. |
| FR-015 | Detection consequence | Being spotted applies a controlled Resolve penalty without continuous frame-by-frame draining. |
| FR-016 | Feedback | Hazard and detection events produce understandable visual or interface feedback. |

## 8.3 Resolve and Checkpoints

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-017 | Resolve initialization | Resolve starts at the configured maximum. |
| FR-018 | Resolve reduction | Damage reduces Resolve by the configured amount. |
| FR-019 | Resolve restoration | Designated collectibles restore Resolve only when configured to do so. |
| FR-020 | Resolve bounds | Resolve remains between zero and its configured maximum. |
| FR-021 | Resolve interface | The HUD reflects Resolve changes immediately. |
| FR-022 | Checkpoint activation | Designated checkpoints record the correct respawn position and associated state. |
| FR-023 | Zero-Resolve recovery | Reaching zero triggers a controlled recovery sequence. |
| FR-024 | Recovery completion | Mary returns to the latest active checkpoint with the configured Resolve amount. |
| FR-025 | Recovery safety | Recovery does not cause softlocks, duplicate rewards, or immediate repeated damage. |

## 8.4 Collectibles and Inventory

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-026 | Key collection | Mary can collect the buried key once. |
| FR-027 | Key requirement | The hidden door cannot be unlocked before the required key is obtained. |
| FR-028 | Seed collection | Seeds can be collected and counted. |
| FR-029 | Flower collection | Flowers can be collected and counted. |
| FR-030 | Unique collection state | Each collectible has a reliable unique identity or collection state. |
| FR-031 | Collection feedback | Collecting an item updates its visual state and the relevant interface. |
| FR-032 | Recovery consistency | Collected items cannot be duplicated by checkpoint recovery. |

## 8.5 Clues and Objectives

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-033 | Clue interaction | Designated clues can be examined or activated. |
| FR-034 | Clue feedback | Clues display useful information or dialogue. |
| FR-035 | Objective tracking | The game tracks required objectives using explicit completion conditions. |
| FR-036 | Objective interface | The current objective is displayed clearly. |
| FR-037 | Door interaction | The hidden door responds appropriately to E. |
| FR-038 | Area unlocking | Areas unlock only when their prerequisites are satisfied. |
| FR-039 | Progression feedback | Completing objectives communicates the next action or milestone. |

## 8.6 Garden Restoration

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-040 | Forgotten state | The initial garden has a neglected visual appearance. |
| FR-041 | Growing state | Completed restoration objectives cause appropriate visual changes. |
| FR-042 | Blooming state | The final garden has vibrant, healthy vegetation and a welcoming appearance. |
| FR-043 | Restoration conditions | Restoration points activate only when their required conditions are satisfied. |
| FR-044 | Restoration persistence | Completed restoration changes persist according to the approved persistence rules. |
| FR-045 | Restoration feedback | Each restoration action produces visible feedback. |
| FR-046 | Final unlock condition | The final garden cannot unlock before all required restoration objectives are complete. |
| FR-047 | Ending condition | The ending triggers only after the approved final objective is completed. |

## 8.7 User Interface and Game Flow

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-048 | Main menu | The main menu provides the game title, Start Game, Instructions, and Exit Game. |
| FR-049 | Continue option | Continue appears only if a functional save system is implemented. |
| FR-050 | Gameplay HUD | The HUD displays Resolve, the current objective, and relevant collectible counts. |
| FR-051 | Interaction prompt | Eligible interactable objects can display an interaction prompt. |
| FR-052 | Pause menu | Resume, Restart from the appropriate starting point, and Return to Main Menu work as specified. |
| FR-053 | Notifications | Important clues, collection events, locked doors, recovery, and restoration milestones display feedback. |
| FR-054 | Ending screen | The ending displays the completed garden and a concluding narrative message. |

## 8.8 Saving and Documentation

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| FR-055 | Save progress | If implemented, saving records the approved persistent progress fields. |
| FR-056 | Load validation | Missing or invalid save data is handled safely. |
| FR-057 | Continue functionality | A valid saved game can be resumed when Continue is selected. |
| FR-058 | Test traceability | Requirements link to implementation tasks and relevant tests. |
| FR-059 | Evidence-based reporting | Test results and screenshots correspond to the actual implementation. |
| FR-060 | Final documentation | The final documentation reports implemented features, actual test results, and known limitations. |

---

# 9. Non-Functional Requirements

| ID | Requirement | Acceptance criterion |
| --- | --- | --- |
| NFR-001 | Maintainability | Scripts have clear responsibilities and avoid unnecessary coupling. |
| NFR-002 | Readability | Code uses meaningful names and appropriate comments. |
| NFR-003 | Project compatibility | Changes work with the inspected Unity version and installed packages. |
| NFR-004 | Responsiveness | Keyboard input responds promptly during normal gameplay. |
| NFR-005 | Collision reliability | Normal movement does not allow Mary to pass through solid level geometry. |
| NFR-006 | State consistency | Resolve, collectibles, objectives, checkpoints, and restoration use consistent state rules. |
| NFR-007 | Recovery reliability | Resolve recovery does not permanently prevent progression. |
| NFR-008 | Performance | The game targets smooth 60 FPS at the approved PC test configuration, subject to documented hardware limitations. |
| NFR-009 | Usability | Controls, objectives, notifications, and menus are understandable. |
| NFR-010 | Asset compliance | Assets are original, licensed, or otherwise appropriately permitted. |
| NFR-011 | Testability | Core game systems can be tested independently where practical. |
| NFR-012 | Data safety | Optional saved progress is validated before use. |
| NFR-013 | Scope control | No combat, weapons, multiplayer, or unrelated systems are added. |
| NFR-014 | Documentation accuracy | Unimplemented or unverified features are never presented as complete. |
| NFR-015 | Regression safety | Changes do not break previously verified functionality. |

Performance must be measured in the actual project; the target is not a claim that the game already achieves 60 FPS.

---

# 10. Measurable Gameplay Rules

The following values are proposed initial tuning targets, not verified facts or mandatory final values. They must be confirmed during technical design and playtesting.

| System | Proposed initial target | Verification |
| --- | --- | --- |
| Horizontal movement | Approximately 5 Unity units per second | Measure displacement over a timed interval. |
| Jump | Approximately 4.5 Unity units per second initial vertical velocity, subject to gravity tuning | Measure jump height and landing behavior. |
| Hazard damage | One configured damage event per contact episode, with cooldown protection | Hold Mary against a hazard and inspect Resolve changes. |
| NPC detection | A defined detection region and configurable patrol speed | Test detection entry, continued overlap, and exit. |
| Resolve | Start at 100; clamp between 0 and 100 | Test initialization, reduction, restoration, and boundaries. |
| Checkpoint recovery | Restore a configured Resolve value and respawn at the latest activated checkpoint | Trigger recovery at multiple checkpoints. |
| Collectibles | One successful collection per unique item ID | Interact repeatedly and verify that rewards are not duplicated. |
| Locked door | Requires the designated key | Attempt entry before and after collecting the key. |
| Restoration | Each area changes state only after its configured prerequisites are satisfied | Compare visuals and state before and after activation. |
| Ending | Requires all mandatory final objectives | Attempt to trigger the ending with incomplete and complete objectives. |

### 10.1 Movement Test Parameters

The implementation plan must define and record:

- Horizontal speed.
- Acceleration and deceleration, if used.
- Jump velocity or equivalent jump configuration.
- Gravity behavior.
- Maximum jump height.
- Air control.
- Ground detection method.
- Collision layers and masks.
- Coyote time or jump buffering, if included.
- Maximum safe platform height and gap width for the approved test level.

The values must be tuned for the actual sprites, collision geometry, and level scale.

### 10.2 Resolve Damage Rules

The preferred baseline is:

- Hazards cause discrete damage events rather than frame-by-frame damage.
- NPC detection causes a controlled penalty on detection entry or another explicitly approved event.
- Sustained contact does not drain Resolve continuously unless a deliberate, documented mechanic is approved.
- Resolve never falls below zero or exceeds its maximum.
- Reaching zero triggers checkpoint recovery.
- Recovery restores a configured Resolve amount and protects Mary from immediate repeated damage while she is repositioned.

### 10.3 Checkpoint Persistence Rules

The proposed baseline is:

- Collected items remain collected after checkpoint recovery.
- Completed objectives remain completed.
- Garden restoration progress remains completed.
- The buried key remains collected once obtained.
- Mary respawns at the latest activated checkpoint.
- Resolve is restored to a configured recovery value.
- Nearby hazards and NPCs are reset or repositioned safely where required.
- One-time rewards cannot be granted repeatedly.

This persistence policy must be implemented consistently across the game. A full game restart may follow different rules from checkpoint recovery; that distinction must be documented.

### 10.4 NPC Detection Rules

Each NPC must have configurable:

- Patrol points or patrol boundaries.
- Movement speed.
- Detection range or trigger area.
- Detection entry conditions.
- Detection exit conditions.
- Resolve penalty.
- Recovery or reset behavior.

Line-of-sight requirements, if any, must be decided during technical design. NPCs must not detect Mary indefinitely after she leaves their defined detection conditions.

---

# 11. Garden Restoration State Model

The garden uses three primary visual states.

## 11.1 Forgotten Garden

- Muted or grayish colors.
- Dead or dormant vegetation.
- Overgrown pathways.
- Tangled brambles.
- Abandoned atmosphere.

## 11.2 Growing Garden

- New plants appear.
- Some flowers bloom.
- Selected areas regain color.
- Approved paths may become accessible.
- Restoration progress is visually distinguishable.

## 11.3 Blooming Garden

- Vibrant colors.
- Healthy plants and flowers.
- Welcoming pathways.
- Optional ambient effects such as drifting leaves or butterflies.

## 11.4 Restoration Rules

1. Restoration must be tied to explicit objective conditions.
2. Each restoration point must have a unique identity.
3. Activating a completed restoration point must not duplicate rewards.
4. Visual changes must correspond to stored game state.
5. Restored paths must have matching collision and traversal behavior.
6. The final state cannot be reached by bypassing required objectives.
7. The ending cannot trigger merely because the player enters the final area.

The technical design should prefer a simple data-driven state model using appropriate Unity components, prefabs, ScriptableObjects, or tilemap changes. Do not introduce unnecessary architectural complexity.

---

# 12. Proposed Unity Architecture

This is a proposed architecture subject to inspection and approval.

| Component or system | Responsibility |
| --- | --- |
| PlayerController | Horizontal movement, jumping, and player input. |
| GroundDetector | Ground and platform detection. |
| PlayerAnimationController | Synchronizes animation with movement state. |
| Interactable | Common interaction contract for designated objects. |
| InteractionController | Finds and activates eligible nearby interactions. |
| Hazard | Defines hazard behavior and damage events. |
| ResolveSystem | Manages Resolve values and recovery triggers. |
| NPCPatrol | Implements predictable patrol behavior. |
| NPCDetection | Handles detection regions and detection state. |
| Checkpoint | Records checkpoint identity and respawn position. |
| CheckpointManager | Tracks the active checkpoint and performs recovery. |
| Collectible | Defines collectible identity and collection behavior. |
| InventorySystem | Tracks collected items and relevant counts. |
| ObjectiveManager | Tracks prerequisites and objective completion. |
| DoorController | Enforces key and objective requirements. |
| ClueInteractable | Presents clue text or dialogue. |
| GardenRestorationPoint | Activates a designated restoration objective. |
| GardenStateController | Applies forgotten, growing, and blooming visual states. |
| GameStateManager | Coordinates game flow and approved global state. |
| HUDController | Displays Resolve, objectives, counts, and notifications. |
| PauseMenuController | Handles pause, resume, restart, and return to menu. |
| SaveSystem | Optional validated save and load functionality. |

These are logical responsibilities, not a requirement to create one script for every row. Reuse existing suitable components and combine responsibilities where that is simpler and safer.

Avoid creating a large, tightly coupled manager that controls every system.

---

# 13. User Interface Requirements

## 13.1 Main Menu

Must include:

- Game title.
- Start Game.
- Instructions.
- Exit Game.
- Continue only if saving is implemented and a valid save is available.

## 13.2 Gameplay HUD

Must display:

- Resolve meter.
- Current objective.
- Relevant collectible counts.
- Interaction prompts when appropriate.
- Important notifications.

## 13.3 Pause Menu

Must include:

- Resume.
- Restart from the appropriate starting point.
- Return to Main Menu.

Pausing must stop relevant gameplay simulation and prevent unintended player movement.

## 13.4 Dialogue and Notifications

The interface must communicate:

- Important clues.
- Collectible discoveries.
- Locked-door requirements.
- Objective completion.
- Checkpoint recovery.
- Garden restoration milestones.

Messages should be concise, readable, and consistent.

## 13.5 Ending Screen

The ending screen must:

- Show the fully blooming garden.
- Provide a concluding narrative message.
- Reinforce hope, friendship, care, discovery, and personal growth.
- Appear only after all required final objectives are complete.

---

# 14. Save System Scope

Saving is optional and must not take priority over core gameplay.

If approved for implementation, saved progress should include the relevant subset of:

- Unlocked areas.
- Key collection status.
- Collected item identities.
- Completed objectives.
- Garden restoration states.
- Checkpoint information, where appropriate.

The implementation must validate loaded data and safely handle missing, outdated, or invalid save files.

If saving is deferred:

- Document it as a future enhancement.
- Do not display a functional Continue option.
- Do not claim persistent progress across game restarts.
- Continue to support the approved checkpoint recovery rules during gameplay.

---

# 15. Testing and Verification Plan

Testing must begin with movement and continue in dependency order.

## 15.1 Required Test Cases

| Test ID | Requirement IDs | Test | Expected result |
| --- | --- | --- | --- |
| TC-001 | FR-002 | Move left and right | Mary moves in the correct direction. |
| TC-002 | FR-003, FR-004, FR-006 | Jump and land on platforms | Jumping, falling, and landing behave correctly. |
| TC-003 | FR-005 | Move against solid geometry | Mary does not pass through the geometry. |
| TC-004 | FR-009–FR-011 | Contact a hazard | Resolve decreases according to the damage rule without frame-by-frame draining. |
| TC-005 | FR-012–FR-016 | Enter and leave NPC detection | Detection starts and ends according to its configured conditions. |
| TC-006 | FR-017–FR-021 | Reduce and restore Resolve | Values and HUD remain synchronized and within bounds. |
| TC-007 | FR-022–FR-025 | Trigger checkpoint recovery | Mary respawns safely at the latest checkpoint. |
| TC-008 | FR-026, FR-030–FR-032 | Collect the buried key | The key is collected once and duplicate rewards are prevented. |
| TC-009 | FR-027, FR-037 | Interact with the hidden door | The door remains locked without the key and unlocks when requirements are met. |
| TC-010 | FR-028–FR-032 | Collect seeds and flowers | Counts and item states update correctly. |
| TC-011 | FR-033–FR-039 | Read clues and complete objectives | Clues provide useful information and progression updates correctly. |
| TC-012 | FR-038 | Attempt to enter a locked area | The area remains inaccessible until its prerequisites are satisfied. |
| TC-013 | FR-040–FR-045 | Activate restoration points | The garden visibly changes and completed restoration persists. |
| TC-014 | FR-046–FR-047 | Attempt to trigger the ending early | The ending remains unavailable until all required objectives are complete. |
| TC-015 | FR-048–FR-054 | Test menus, HUD, and pause | All implemented controls and interface elements behave correctly. |
| TC-016 | FR-055–FR-057 | Restart and continue | If saving is implemented, valid progress is restored safely. |
| TC-017 | NFR-008 | Measure performance | Performance is recorded against the approved target configuration. |
| TC-018 | NFR-015 | Run regression tests | Previously verified features remain functional. |

## 15.2 Test Record Format

Every executed test must document:

- Test ID.
- Requirement IDs.
- Test date.
- Build or project revision.
- Preconditions.
- Test steps.
- Expected result.
- Actual result.
- Status.
- Defect or issue reference, if applicable.
- Evidence screenshot or log, where relevant.

Allowed statuses:

- NOT RUN.
- BLOCKED.
- PASS.
- FAIL.
- RETEST REQUIRED.

A test can be marked PASS only after execution confirms the expected result.

## 15.3 Testing Methods

Use:

- Unity Test Framework for suitable automated tests.
- Play Mode tests for gameplay behavior.
- Edit Mode tests for independent logic where practical.
- Manual playtesting for controls, level flow, presentation, and usability.
- Build testing for the intended PC target.
- Regression testing after changes to shared systems.

If a test cannot be executed in the current environment, record it as NOT RUN or BLOCKED and explain why.

---

# 16. Requirement-to-Test Traceability

Maintain a dedicated traceability matrix throughout development.

The initial requirement IDs and test mappings are defined in Sections 8 and 15.

For each requirement, record:

| Field | Description |
| --- | --- |
| Requirement ID | Unique FR or NFR identifier. |
| Requirement summary | Brief description of required behavior. |
| Approval status | PROPOSED, APPROVED, or the applicable status. |
| Implementation task | Linked task identifier. |
| Implementation location | Actual script, scene, prefab, or asset path. |
| Test ID | Linked verification case. |
| Test status | NOT RUN, BLOCKED, PASS, or FAIL. |
| Evidence | Actual screenshot, test output, or log reference. |
| Notes | Known issues, limitations, or change history. |

The matrix must be updated whenever requirements, code, scenes, or tests change.

Do not invent file paths before files exist.

---

# 17. Phased Implementation Plan

The following sequence is a planning baseline. It must be refined and approved after project inspection and requirements approval.

## Phase 1: Requirements Analysis

- Inspect the project.
- Document the current state.
- Finalize the game specification.
- Define acceptance criteria and open questions.
- Present the specification for approval.

**Exit condition:** The user approves the specification.

## Phase 2: Technical Design

- Define scene structure.
- Define component responsibilities.
- Define input handling.
- Define collision layers and physics configuration.
- Define persistent and temporary game state.
- Define test organization.
- Create implementation tasks and dependencies.

**Exit condition:** The user approves the technical design and task plan.

## Phase 3: Core Movement

- Implement horizontal movement.
- Implement jumping.
- Implement ground detection.
- Implement collision behavior.
- Implement basic animation states.
- Create a basic test level.
- Test keyboard-only completion of the test level.

**Exit condition:** Movement and collision acceptance criteria pass.

## Phase 4: Hazards and NPC Detection

- Implement hazard collision.
- Implement controlled damage events.
- Implement NPC patrol.
- Implement detection and detection exit conditions.
- Add visual feedback.
- Test all relevant behaviors.

**Exit condition:** Hazard and detection tests pass.

## Phase 5: Resolve and Checkpoints

- Implement Resolve initialization and bounds.
- Implement damage and restoration.
- Implement HUD updates.
- Implement checkpoint activation.
- Implement zero-Resolve recovery.
- Test persistence and safe repositioning.

**Exit condition:** Resolve and recovery tests pass.

## Phase 6: Collectibles and Interactions

- Implement unique collectible identities.
- Implement key, seed, and flower collection.
- Implement inventory counts.
- Implement clue interaction.
- Implement the locked door.
- Test collection and interaction rules.

**Exit condition:** Collection, inventory, clue, and door tests pass.

## Phase 7: Objectives and Progression

- Implement objective tracking.
- Implement prerequisites.
- Implement area unlocking.
- Connect progression to the story chapters.
- Test valid and invalid progression attempts.

**Exit condition:** Progression cannot bypass required prerequisites.

## Phase 8: Garden Restoration

- Implement restoration points.
- Implement forgotten, growing, and blooming states.
- Connect restoration to objective completion.
- Persist restored state as approved.
- Test visuals, collision changes, and progression.

**Exit condition:** Restoration and final unlock tests pass.

## Phase 9: User Interface and Game Flow

- Implement the main menu.
- Implement instructions.
- Implement the gameplay HUD.
- Implement pause and restart behavior.
- Implement dialogue and notifications.
- Implement the ending screen.

**Exit condition:** UI and game-flow tests pass.

## Phase 10: Optional Saving and Polish

- Implement validated save and load if approved.
- Add licensed or original audio.
- Improve animations and transitions.
- Add optional ambient effects.
- Perform usability and performance checks.

**Exit condition:** Optional additions do not break core gameplay.

## Phase 11: Final Verification

- Execute the full test suite.
- Perform regression testing.
- Build and run the target PC version.
- Record actual results.
- Capture real screenshots.
- Document known limitations.
- Prepare the final report.

**Exit condition:** Results are accurately documented and the user approves project completion.

---

# 18. Asset and Content Policy

Use original, licensed, or appropriately permitted assets.

Temporary assets are allowed during development when final artwork is unavailable.

Temporary content must:

- Be identifiable as temporary.
- Support actual gameplay testing.
- Be replaceable without rewriting unrelated systems.
- Not be presented as final artwork.

Do not generate fake screenshots or claim that planned assets exist.

Do not replace user-created assets without authorization.

---

# 19. Out-of-Scope Features

The following are excluded unless the user explicitly approves a specification change:

- Combat.
- Weapons.
- Enemy defeat mechanics.
- Multiplayer.
- Online services.
- Unrelated crafting systems.
- Complex skill trees.
- Unrelated side quests.
- Unnecessary procedural world generation.
- Unnecessary external dependencies.
- Paid asset purchases without approval.
- Features that do not support the story, exploration, or restoration objectives.

Optional audio, visual effects, and save functionality are secondary to the required gameplay systems.

---

# 20. Known Risks and Open Decisions

Cursor must investigate and document these items during requirements analysis.

| ID | Decision or risk | Required action |
| --- | --- | --- |
| OD-001 | Existing Unity project status | Inspect the project before deciding what to create. |
| OD-002 | Unity version and packages | Determine compatible APIs and input configuration. |
| OD-003 | Target resolution and aspect ratio | Propose a PC presentation target for approval. |
| OD-004 | Movement tuning | Validate proposed values in the actual test level. |
| OD-005 | Resolve damage amounts | Establish consistent hazard and NPC penalties. |
| OD-006 | Checkpoint persistence | Confirm the proposed rule that collected items and completed objectives persist through recovery. |
| OD-007 | NPC detection | Determine whether detection requires range alone or line of sight. |
| OD-008 | Loose stones | Clarify whether they are stationary hazards, moving obstacles, or both. |
| OD-009 | Restoration prerequisites | Define exact item counts and objectives required for each restoration point. |
| OD-010 | Chapter structure | Decide whether chapters use separate scenes or connected level sections. |
| OD-011 | Save functionality | Confirm whether it is required for the initial submission. |
| OD-012 | Final artwork and audio | Identify available assets and any licensing requirements. |
| OD-013 | Testing environment | Confirm available editor, hardware, build target, and automated testing support. |

Cursor must not silently resolve decisions that materially affect gameplay or project scope.

Where a sensible default is sufficient, document the recommendation and obtain approval as required.

---

# 21. Final Documentation and Evidence

The final report must include:

1. Project overview.
2. Game objectives.
3. Story and narrative design.
4. Character descriptions.
5. Environment descriptions.
6. Gameplay mechanics.
7. Controls.
8. Functional and non-functional requirements.
9. Architecture and implementation details.
10. Development process.
11. Test plan and actual test results.
12. Requirements-to-tests traceability.
13. Known limitations.
14. Future improvements.
15. Screenshots and captions.

## 21.1 Required Screenshots

Capture actual screenshots of the implemented game showing:

1. Main menu.
2. Mary moving and jumping.
3. Hazards and NPC interactions.
4. Resolve meter.
5. Buried key collection.
6. Secret garden entrance.
7. Forgotten garden.
8. Garden restoration in progress.
9. Fully blooming garden.
10. Ending screen.

Each screenshot must include a caption and a brief explanation of the feature demonstrated.

If a feature has not been implemented or cannot be reached in the current build, mark the corresponding screenshot as pending.

Do not create fabricated evidence or use concept art as proof of implementation.

---

# 22. Progress Tracking

Maintain a development log with the following fields:

| Field | Purpose |
| --- | --- |
| Phase | Current development phase. |
| Task ID | Unique implementation task identifier. |
| Requirement IDs | Requirements addressed by the task. |
| Changes | Files and assets actually changed. |
| Dependencies | Required prior tasks. |
| Tests executed | Actual test identifiers. |
| Test results | Observed outcomes. |
| Evidence | Actual screenshots, logs, or test output. |
| Status | Pending, In Progress, Blocked, Implemented, Verified, or Failed. |
| Next action | The next approved task. |

Every implementation response from Cursor should explain:

1. What was implemented.
2. Why the change is needed.
3. Which requirement IDs it addresses.
4. Which files were created or modified.
5. How the feature can be run and tested.
6. Which tests were executed.
7. The actual test results.
8. Any unresolved issues.
9. Whether the phase exit criteria are satisfied.
10. The next proposed task.

Only list files that actually exist or were actually modified.

---

# 23. Mandatory First Task

When this README is provided to Cursor, begin with **Phase 1: Requirements Analysis**.

Perform the following actions in order:

1. Inspect the existing project context and files.
2. Identify the Unity version, project structure, and available assets.
3. Report findings without modifying project files.
4. Review the proposed game design and all requirements.
5. Identify contradictions, missing decisions, dependencies, and risks.
6. Present the complete Game Design and Software Requirements Specification.
7. Present measurable acceptance criteria.
8. Present the requirements-to-tests traceability matrix.
9. Present the recommended technical architecture.
10. Present the proposed phased implementation plan.
11. Clearly list the decisions requiring approval.
12. Wait for the user's explicit approval.

**Do not generate game code, create scenes, modify assets, install packages, or change project settings during this first phase.**

If project inspection reveals that a requirement is incompatible with the existing configuration, explain the problem and propose an approved resolution.

If no Unity project exists, report that fact and ask the user to create or select a Unity project before implementation.

Do not assume that the existence of this README means the project is ready for development.

---

# 24. Definition of Project Completion

The project may be described as complete only when:

- The approved functional requirements are implemented or formally deferred with authorization.
- The main gameplay loop works.
- Movement and collision behavior pass their acceptance tests.
- Hazards, NPC detection, Resolve, and checkpoint recovery work as specified.
- Collectibles and interactions enforce their requirements.
- Objective tracking and area unlocking behave correctly.
- Garden restoration visibly progresses through the approved states.
- The final ending cannot be triggered before its prerequisites are satisfied.
- Menus and the HUD behave correctly.
- Required tests have been executed and accurately reported.
- Regression testing has been performed.
- The target PC build has been tested to the extent supported by the available environment.
- Screenshots represent the actual implementation.
- Known limitations are documented.
- The final documentation and traceability matrix are updated.

Optional features must be clearly distinguished from required features.

A feature that has been coded but not verified must not be described as fully working.

---

# 25. Final Instruction to Cursor

Treat this project as a controlled, incremental software engineering process rather than a one-shot code generation task.

The specification defines what the game must do. The approved technical design defines how the systems should be structured. The implementation tasks define the work to perform. The tests provide evidence that the requirements have been satisfied.

Inspect first. Specify second. Obtain approval. Plan carefully. Implement incrementally. Test every dependency. Preserve working features. Document actual results.

**Start now with project inspection and requirements analysis only. Present the complete specification for review, and wait for explicit approval before proceeding to implementation planning.**

## How to use this README in Cursor

1. Open your Unity project root in Cursor.
2. Save the document above as `README.md`.
3. Open Cursor Agent and tell it to read `README.md` and perform Section 23 only.
4. Review the specification Cursor produces. Correct anything that conflicts with your instructor's requirements.
5. Approve the specification before asking Cursor to prepare implementation tasks.

Important: Your README is the project's development contract, not an automatic guarantee that Cursor will implement every feature correctly. Keep changes incremental, review proposed file edits, and run the actual Unity project and tests after each milestone.
