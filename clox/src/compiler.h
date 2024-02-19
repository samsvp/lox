#ifndef clox_compiler_h
#define clox_compiler_h

#include <stdbool.h>

#include "vm.h"
#include "object.h"

// compiles the source into the given chunk.
// returns true if code has been successfully compiled
// and false otherwise (e.g. if it encountered syntax errors)
ObjFunction* compile(const char* source);

#endif
