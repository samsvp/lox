#ifndef clox_debug_h
#define clox_debug_h

#include "chunk.h"

// prints a chunk of instructions as our VMs assembly equivalent
void disassembleChunk(Chunk* chunk, const char* name);
// prints an instruction as our VMs assembly equivalent.
// returns the offset of the next instruction
int disassembleInstruction(Chunk* chunk, int offset);

#endif 
