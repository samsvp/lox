#ifndef CLOX_MEMORY_H
#define CLOX_MEMORY_H

#include "common.h"
#include "object.h"


#define ALLOCATE(type, count) \
    (type*)reallocate(NULL, 0, sizeof(type) * count)

#define FREE(type, pointer) reallocate(pointer, sizeof(type), 0)

#define GROW_CAPACITY(cpcty) ((cpcty) < 8 ? 8 : cpcty * 2)

#define GROW_ARRAY(type, ptr, oldCount, newCount) \
    (type*) reallocate(ptr, sizeof(type) * (oldCount), \
        sizeof(type) * (newCount))

#define FREE_ARRAY(type, ptr, oldCount) \
    reallocate(ptr, sizeof(type) * (oldCount), 0)

void* reallocate(void* ptr, size_t oldSize, size_t newSize);
void freeObjects();

#endif