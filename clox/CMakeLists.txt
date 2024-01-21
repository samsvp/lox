cmake_minimum_required(VERSION 3.5)
project(lox)

# Add main.c file
add_executable(main main.c)

# Add source files from src directory
set(SOURCE_FILES
    src/chunk.c
    src/debug.c
    src/memory.c
    src/value.c
)

# Add header files from src directory
set(HEADER_FILES
    src/chunk.h 
    src/common.h 
    src/debug.h
    src/memory.h 
    src/value.h
)

# Add the source files to the executable
target_sources(main PRIVATE ${SOURCE_FILES} ${HEADER_FILES})
