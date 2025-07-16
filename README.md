# What is for this library?

I made this library mainly to continue learning and creating cheats in games, C# is a very powerful language but when it comes to write and read memory is very annoying, more when you have several projects, so with this library my intention is to make this process more comfortable for me.

This library is the one I will use in my next external cheat projects in C#.

# Anticheat evasion

Right now this way of writing to memory is highly detected by anticheats, as it is the most basic way using `OpenProcess`, in the future I intend to update the library so that you can do HandleHijacking by duplicating a handle that has permissions on the game process in order to bypass most client anticheats.

# How to use it

These are the main functions of the library, there are also variants not only for integrals, but also for booleans, chars, etc. If you want to use it more in depth I recommend you to take a look at the code, it is all documented in it.

## Process functions

- SetProcess function to find the process that has that name
  
  `rug4lo rug4lo = new rug4lo("game");`

- GetModuleBase function to set the module base
  
  `IntPtr client = rug4lo.GetModuleBase("game.dll");`

## Pointer reading functions

- ReadPointer to read the data from a pointer (it is prepared to receive offsets as in the example)
  
  `IntPtr example = rug4lo.ReadPointer(client, 0x1A044E0);`
  
  `IntPtr example2 = rug4lo.ReadPointer(client, 0x1A044E0 * 0x78);`
  
  `IntPtr example3 = rug4lo.ReadPointer(client, 0x8 *((0x1A044E0 & 0x7FFF) >> 9) + 0x10);`

## Memory read functions

- ReadShort to read the data at a memory address and store it as a short data type
  
  `short example= rug4lo.ReadShort(0x30, 0x824);`

- ReadInt to read the data at a memory address and store it as an Int data type
  
  `int example= rug4lo.ReadInt(0x30, 0x824);`

- ReadLong to read the data at a memory address and store it as a Long data type
  
  `long example= rug4lo.ReadLong(0x30, 0x824);`

- ReadFloat to read the data at a memory address and store it as a Float data type
  
  `float example= rug4lo.ReadFloat(0x30, 0x824);`

- ReadBool to read the data at a memory address and store it as a Bool data type
  
  `bool example= rug4lo.ReadBool(0x30, 0x824);`

## Unsigned memory read functions

- ReadUInt to read the data at a memory address and store it as a UInt data type
  
  `uint example= rug4lo.ReadUInt(0x30, 0x824);`

## Memory Write functions

- WriteShort to write the Short data at a memory address (returns a boolean type data)
  
  `rug4lo.WriteShort(0x30, 0x824 + 0x51, 1);`

- WriteInt to write the Int data at a memory address
  
  `rug4lo.WriteInt(0x30, 0x824 + 0x51, 1);`

- WriteLong to write the Long data at a memory address
  
  `rug4lo.WriteLong(0x30, 0x824 + 0x51, 1);`

- Writebool to write the Boolean data at a memory address
  
  `rug4lo.WriteBool(0x30, 0x824 + 0x51, True);`

## Unsigned memory read functions

- WriteUInt to write the UInt data at a memory address
  
  `rug4lo.WriteUInt(0x30, 0x824 + 0x51, Color);`
