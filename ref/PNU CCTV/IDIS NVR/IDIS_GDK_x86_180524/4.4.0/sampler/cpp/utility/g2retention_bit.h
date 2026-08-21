// g2retention_bit.h : header file
//

#ifndef _G2_CLIENT_DLL_UTILITY_RETENTION_BIT_H_
#define _G2_CLIENT_DLL_UTILITY_RETENTION_BIT_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace g2 {

//////////////////////////////////////////////////////////////////////////

inline size_t bits_to_bytealign(size_t bits, size_t bytes)
{
    const size_t hope = (bytes << 3) - 1;
    return ((bits + hope) & ~hope) >> 3;
}

template <typename T> inline
size_t countof_bits(T data)
{
    size_t count = 0;
    if (data) {
        do { ++count; } while (data &= (data - 1));
    }
    return count;
}

template <typename T> inline
bool contains_bit(const T& data, size_t loc)
{
    static const T indexer = 0x1;
    static const size_t limit = sizeof(T) << 3;
    return ((loc < limit) &&
            (data & (indexer << loc)));
}

template <typename T> inline
bool contains_bitor(const T& data, const T& bits)
{
    return ((data & bits) != 0);
}

template <typename T> inline
bool contains_bitand(const T& data, const T& bits)
{
    return ((data & bits) == bits);
}

template <typename T> inline
void set_bit(T& data, size_t loc)
{
    static const T indexer = 0x1;
    static const size_t limit = sizeof(T) << 3;
    if (loc < limit) {
        data |= (indexer << loc);
    }
}

template <typename T> inline
void set_bits(T& data, const T& area, size_t loc = 0)
{
    data |= (area << loc);
}

template <typename T> inline
void reset_bit(T& data, size_t loc)
{
    static const T indexer = 0x1;
    static const size_t limit = sizeof(T) << 3;
    if (loc < limit) {
        data &= ~(indexer << loc);
    }
}

template <typename T> inline
void reset_bits(T& data, const T& area, size_t loc = 0)
{
    data &= ~(area << loc);
}

template <typename T> inline
void invert_bit(T& data, size_t loc)
{
    static const T indexer = 0x1;
    static const size_t limit = sizeof(T) << 3;
    if (loc < limit) {
        data ^= (indexer << loc);
    }
}

template <typename T> inline
void invert_bits(T& data, const T& area, size_t loc)
{
    data ^= (area << loc);
}

template <typename T> inline
T extract_bits(const T& data, const T& area, size_t loc)
{
    return (data >> loc) & area;
}

template <typename T> inline
int find_first_biton(const T& data)
{
    static const size_t limit = sizeof(T) << 3;
    int index = -1;
    for (int i = 0; i < limit; ++i) {
        if (contains_bit(data, i)) {
            index = i;
            break;
        }
    }
    return index;
}

template <typename T> inline
int find_first_bitoff(const T& data)
{
    static const size_t limit = sizeof(T) << 3;
    int index = -1;
    for (int i = 0; i < limit; ++i) {
        if (!contains_bit(data, i)) {
            index = i;
            break;
        }
    }
    return index;
}

template <typename T> inline
int find_biton(const T& data, int distance)
{
    static const size_t limit = sizeof(T) << 3;
    int index = -1;
    for (int i = 0; i < limit; ++i) {
        if (contains_bit(data, i)) {
            if (--distance < 0) {
                index = i;
                break;
            }
        }
    }
    return index;
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_g2

#endif // !_G2_CLIENT_DLL_UTILITY_RETENTION_BIT_H_
