// g2channels.inl : inline implementation file
//

#ifndef _G2_CLIENT_DLL_UTILITY_CHANNELS_INL_
#define _G2_CLIENT_DLL_UTILITY_CHANNELS_INL_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <utility/g2retention_bit.h>

#ifndef _G2_CLIENT_DLL_UTILITY_CHANNELS_H_
#error g2channels.inl requires g2channels.h to be included first
#endif

namespace g2 {

//////////////////////////////////////////////////////////////////////////

inline std::set<int> channels::to_channelset(void) const
{
    std::set<int> channelset;
    for (int i = 0; i <= MASK_MAX_CHANNEL; ++i) {
        if (contains(i)) { channelset.insert(i); }
    }
    return channelset;
}

inline unsigned int channels::count(void) const
{
    return static_cast<unsigned int>(g2::countof_bits(_mask));
}

//////////////////////////////////////////////////////////////////////////

inline bool channels::operator==(const channels& channels) const
{
    return (_mask == channels._mask);
}

inline bool channels::operator!=(const channels& channels) const
{
    return !(operator == (channels));
}

inline channels& channels::operator+=(const channels& channels)
{
    _mask |= channels._mask;
    return (*this);
}

inline channels& channels::operator-=(const channels& channels)
{
    _mask &= (~(channels._mask));
    return (*this);
}

inline channels& channels::operator|=(const channels& channels)
{
    _mask |= channels._mask;
    return (*this);
}

inline channels& channels::operator&=(const channels& channels)
{
    _mask &= channels._mask;
    return (*this);
}

inline channels operator+(const channels& ch1, const channels& ch2)
{
    return channels(ch1._mask | ch2._mask);
}

inline channels operator-(const channels& ch1, const channels& ch2)
{
    return channels(ch1._mask & (~(ch2._mask)));
}

inline channels operator|(const channels& ch1, const channels& ch2)
{
    return channels(ch1._mask | ch2._mask);
}

inline channels operator&(const channels& ch1, const channels& ch2)
{
    return channels(ch1._mask & ch2._mask);
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_g2

#endif // !_G2_CLIENT_DLL_UTILITY_CHANNELS_INL_
