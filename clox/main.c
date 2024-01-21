#include "src/common.h"
#include "src/chunk.h"
#include "src/debug.h"


int main(int argc, char** argv)
{
    Chunk chunk;
    initChunk(&chunk);
    writeChunk(&chunk, OP_RETURN, 2);

    writeConstant(&chunk, 1.2, 2);

    disassembleChunk(&chunk, "my chunk");
    freeChunk(&chunk);
    return 0;
}
