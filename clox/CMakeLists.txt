cmake_minimum_required(VERSION 3.5)
project(lox)

# Add main.c file
add_executable(main main.c)

# Add source files from src directory
set(SOURCE_FILES
    src/chunk.c
    src/common.c
    src/compiler.c
    src/debug.c
    src/memory.c
    src/scanner.c
    src/value.c
    src/vm.c
    src/object.c
    src/table.c
)

# Add header files from src directory
set(HEADER_FILES
    src/chunk.h
    src/common.h
    src/compiler.h
    src/debug.h
    src/memory.h
    src/scanner.h
    src/value.h
    src/vm.h
    src/object.h
    src/table.h
)

# Add the source files to the executable
target_sources(main PRIVATE ${SOURCE_FILES} ${HEADER_FILES})
