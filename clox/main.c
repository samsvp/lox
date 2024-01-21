#include "src/common.h"
#include "src/chunk.h"
#include "src/debug.h"
#include "src/vm.h"


int main(int argc, char** argv)
{
    initVM();

    Chunk chunk;
    initChunk(&chunk);

    writeConstant(&chunk, 1.2, 2);

    writeConstant(&chunk, 3.4, 2);
    writeChunk(&chunk, OP_ADD, 2);
    writeConstant(&chunk, 5.6, 2);
    writeChunk(&chunk, OP_DIVIDE, 2);

    writeChunk(&chunk, OP_NEGATE, 2);
    writeChunk(&chunk, OP_RETURN, 2);

    disassembleChunk(&chunk, "my chunk");
    interpret(&chunk);
    freeVM();
    freeChunk(&chunk);
    return 0;
}
