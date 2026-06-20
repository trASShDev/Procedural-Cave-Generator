# Procedural Cave Generator
A Unity tool for generating cave-like room systems using connected rooms and tunnel-style paths.

## Features
- Room-based cave generation
- Configurable generation area size
- Adjustable room count range
- Branching path support
- Increasing split chance per generated room
- Multi-branch exits when split chance exceeds 100%
- Inspector-editable room prefab arrays
- Start, normal, and exit room types
- Seed-based generation for repeatable layouts
- Optional generation delay for showcase/debug visualization

## Technologies
- Unity
- C#
- Procedural Generation
- Seeded Randomness

## How It Works
The generator places rooms inside a configurable grid, such as a 100x100 possible room area.  
Starting at one of the 4 corners rooms are connected into a cave-like path, with optional branching based on a split chance value.

The split chance can increase after each spawned room.  
For example, a 120% split chance guarantees one branch if space is available and gives an additional 20% chance for a second branch, allowing three-way paths.

## Purpose
This project was developed to demonstrate seed-based procedural generation and scalable algorithm design.