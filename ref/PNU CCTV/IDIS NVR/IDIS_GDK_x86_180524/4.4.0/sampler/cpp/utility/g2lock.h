// g2lock.h : header file
//

#ifndef _G2_CLIENT_DLL_UTILITY_LOCK_H_
#define _G2_CLIENT_DLL_UTILITY_LOCK_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <boost/noncopyable.hpp>

namespace g2 {

//////////////////////////////////////////////////////////////////////////

class critical_section : private boost::noncopyable
{
public:
    critical_section(void)
    {
        InitializeCriticalSection(&_handle);
    }
    ~critical_section(void)
    {
        DeleteCriticalSection(&_handle);
    }

private:
    mutable CRITICAL_SECTION _handle;

public:
    bool trylock(void) const { return TryEnterCriticalSection(&_handle) == TRUE; }
    void enter(void)   const { return EnterCriticalSection(&_handle); }
    void leave(void)   const { return LeaveCriticalSection(&_handle); }
    void lock(void)    const { return enter(); }
    void unlock(void)  const { return leave(); }
};

//////////////////////////////////////////////////////////////////////////

template<typename T>
class scoped_sectionlock_ : private boost::noncopyable
{
public:
    explicit scoped_sectionlock_(const T& tool, bool lock = true)
        : _tool(tool)
        , _lock(lock)
    {
        if (_lock) {
            _tool.lock();
        }
    }
    ~scoped_sectionlock_(void)
    {
        if (_lock) {
            _tool.unlock();
        }
    }

private:
    const T& _tool;
    mutable volatile bool _lock;

public:
    void lock(void) const
    {
        if (_lock != true) {
            _lock = true;
            _tool.lock();
        }
    }
    void free(void) const
    {
        if (_lock) {
            _tool.unlock();
            _lock = false;
        }
    }

    bool is_locked(void) const { return _lock }
};

//////////////////////////////////////////////////////////////////////////

typedef scoped_sectionlock_<critical_section> scoped_criticalsection;

} // !_namespace_g2

#endif // !_G2_CLIENT_DLL_UTILITY_LOCK_H_
