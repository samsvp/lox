#ifndef clox_chunk_h 
#define clox_chunk_h

#include "common.h"
#include "value.h"
#include <stdint.h>


// note that opcodes may have operands and be more than one byte
typedef enum {
    OP_CONSTANT,
    OP_CONSTANT_LONG,
    OP_NIL, 
    OP_TRUE, 
    OP_FALSE, 
    OP_POP,
    OP_GET_LOCAL,
    OP_SET_LOCAL,
    OP_GET_GLOBAL,
    OP_DEFINE_GLOBAL,
    OP_SET_GLOBAL,
    OP_EQUAL, 
    OP_GREATER, 
    OP_LESS, 
    OP_ADD, 
    OP_SUBTRACT, 
    OP_MULTIPLY, 
    OP_DIVIDE, 
    OP_NOT,
    OP_NEGATE,
    OP_PRINT,
    OP_JUMP,
    OP_JUMP_IF_FALSE,
    OP_LOOP,
    OP_CALL,
    OP_RETURN,
} OpCode;


// this is where the series of instructions will be stored
typedef struct {
    int count;
    int capacity;
    uint8_t* code;
    // line number array
    int* lines;
    // list of constants defined in this chunk of code
    ValueArray constants;
} Chunk;


void initChunk(Chunk* chunk);
// append data to chunk
void writeChunk(Chunk* chunk, uint8_t byte, int line);
// appends a constant. returns the index of the constant in the
// constants array.
int addConstant(Chunk* chunk, Value value);
// add the constant to the array and get the index back. 
// if the index fits in one byte, we use the short opcode 
// and just write the single byte.
void writeConstant(Chunk* chunk, Value value, int line);

void freeChunk(Chunk* chunk);

#endif
