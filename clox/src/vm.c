#include "chunk.h"
#include "common.h"
#include "memory.h"
#include "debug.h"
#include "value.h"
#include "vm.h"
#include <stdint.h>
#include <stdio.h>


VM vm;


static void resetStack() 
{
    vm.stackCount = 0;
}

void initVM()
{
    vm.stack = NULL;
    vm.stackCapacity = 0;
    resetStack();
}


void freeVM()
{

}


void push(Value value)
{
    if (vm.stackCapacity < vm.stackCount + 1) {
        int oldCapacity = vm.stackCapacity;
        vm.stackCapacity = GROW_CAPACITY(oldCapacity);
        vm.stack = GROW_ARRAY(Value, vm.stack, 
                              oldCapacity, vm.stackCapacity);
    }

    vm.stack[vm.stackCount] = value;
    vm.stackCount++;
}


Value pop()
{
    vm.stackCount--;
    return vm.stack[vm.stackCount];
}


static InterpretResult run()
{
#define READ_BYTE() (*vm.ip++)
#define READ_CONSTANT() (vm.chunk->constants.values[READ_BYTE()])
#define BINARY_OP(op) \
    do { \
        double b = pop(); \
        double a = pop(); \
        push( a op b ); \
    } while (false)


    while (true)
    {
#ifdef DEBUG_TRACE_EXECUTION
        printf("       ");
        for (Value *slot = vm.stack; slot < vm.stack + vm.stackCount; slot++) {
            printf("[ ");
            printValue(*slot);
            printf(" ]");
        }
        printf("\n");
        disassembleInstruction(vm.chunk, (int)(vm.ip - vm.chunk->code));
#endif

        uint8_t instruction;
        switch (instruction = READ_BYTE()) {
            case OP_CONSTANT:
            case OP_CONSTANT_LONG: {
                Value constant = READ_CONSTANT();
                push(constant);
                break;
            }      
            case OP_ADD:      BINARY_OP(+); break;
            case OP_SUBTRACT: BINARY_OP(-); break;
            case OP_MULTIPLY: BINARY_OP(*); break;
            case OP_DIVIDE:   BINARY_OP(/); break;
            case OP_NEGATE: {
                vm.stack[vm.stackCount - 1] = -vm.stack[vm.stackCount - 1];
                break;
            }
            case OP_RETURN: {
                printValue(pop());
                printf("\n");
                return INTERPRET_OK;
            }
        }
    }

#undef BINARY_OP
#undef READ_CONSTANT
#undef READ_BYTE 
}


InterpretResult interpret(Chunk *chunk)
{
    vm.chunk = chunk;
    vm.ip = vm.chunk->code;
    return run();
}
