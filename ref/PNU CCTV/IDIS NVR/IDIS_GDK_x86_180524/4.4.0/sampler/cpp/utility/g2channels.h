// g2channels.h : header file
//

#ifndef _G2_CLIENT_DLL_UTILITY_CHANNELS_H_
#define _G2_CLIENT_DLL_UTILITY_CHANNELS_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <set>

namespace g2 {

//////////////////////////////////////////////////////////////////////////

class channels
{
public:
    channels(void)
        : _mask(0x00ui64)
    {
    }
    explicit channels(bool all)
    {
        (all) ? add_all() : remove_all();
    }
    explicit channels(const unsigned __int64& mask)
        : _mask(mask)
    {
    }

    ~channels(void)
    {
    }

public:
    enum { MASK_MAX_CHANNEL = 63 };

protected:
    static const unsigned __int64 _indexer = 0x01ui64;
    unsigned __int64 _mask;  // max 64 channels

public:
    bool empty(void)              const { return (0x00ui64 == _mask); }
    bool contains(unsigned char channel) const { return (_mask & (_indexer << channel)) ? true : false; }

    void add(unsigned char channel)     { _mask |= (_indexer << channel); }
    void add_all(void)                  { _mask = 0xffffffffffffffffui64; }
    void add_mask(const unsigned __int64& mask) { _mask |= mask; }

    void remove(unsigned char channel)  { _mask &= ~(_indexer << channel); }
    void remove_all(void)               { _mask = 0x00ui64;     }

    unsigned __int64 to_uint64(void) const      { return _mask; }
    void from_uint64(const unsigned __int64& i) { _mask = i;    }
    unsigned int to_uint32(void) const  { return static_cast<unsigned int>(_mask); }
    void from_uint32(unsigned int i)    { _mask = static_cast<unsigned __int64>(i); }
    std::set<int> to_channelset(void) const;
    unsigned int count(void) const;

public:
    bool operator==(const channels& channels) const;
    bool operator!=(const channels& channels) const;

    channels& operator+=(const channels& channels);
    channels& operator-=(const channels& channels);
    channels& operator|=(const channels& channels);
    channels& operator&=(const channels& channels);

    friend channels operator+(const channels& ch1, const channels& ch2);
    friend channels operator-(const channels& ch1, const channels& ch2);
    friend channels operator|(const channels& ch1, const channels& ch2);
    friend channels operator&(const channels& ch1, const channels& ch2);
};

//////////////////////////////////////////////////////////////////////////

} // !_namesace_g2

#endif // !_G2_CLIENT_DLL_UTILITY_CHANNELS_H_
