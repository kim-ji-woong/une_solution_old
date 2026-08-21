// g2looper.inl : inline implementation file
//

#ifndef _G2_CLIENT_DLL_UTILITY_LOOPER_INL_
#define _G2_CLIENT_DLL_UTILITY_LOOPER_INL_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#ifndef _G2_CLIENT_DLL_UTILITY_LOOPER_H_
#error g2looper.inl requires g2looper.h to be included first
#endif

#include <process.h>

namespace g2 {

//////////////////////////////////////////////////////////////////////////

template<typename T>
looper_<T>::looper_(void)
    : _handle(NULL)
    , _suspend(NULL)
    , _id(0)
    , _ec(0)
    , _priority(THREAD_PRIORITY_NORMAL)
    , _name()
    , _owner(NULL)
    , _func(NULL)
    , _continue(false)
    , _state(status_::NOT_READY)
{
    _suspend = CreateEvent(NULL, TRUE, FALSE, NULL);
}

template<typename T>
looper_<T>::~looper_(void)
{
    destroy();

    CloseHandle(_suspend);
}

//////////////////////////////////////////////////////////////////////////

template<typename T>
bool looper_<T>::create(T* owner, FUNC func, const std::wstring& name, int priority /*= THREAD_PRIORITY_NORMAL*/, size_t reserve_stack /*= 0*/, bool loop /*= true*/)
{
    assert(owner != NULL && func != NULL);

    if (_handle != NULL) {
        destroy();
    }

    _owner = owner;
    _func = func;
    _priority = priority;
    _name = name;

    unsigned (__stdcall *start_routine)(void*) = (loop) ? core_func_loop : core_func_sole;
    unsigned id = 0;
    unsigned int flags = CREATE_SUSPENDED;
    if (reserve_stack != 0) {
        flags= STACK_SIZE_PARAM_IS_A_RESERVATION;
    }

    HANDLE handle = (HANDLE)::_beginthreadex(NULL, (unsigned)reserve_stack, start_routine, this, flags, &id);

    if (handle) {
        _handle = handle;
        _id = id;
        set_status(status_::READY);
    }
    else {
        set_status(status_::NOT_READY);
        assert(!"failed to create thread");
    }
    return is_valid();
}

template<typename T>
bool looper_<T>::destroy(unsigned long grace_time /*= 10000*/, unsigned long exit_code /*= 0*/)
{
    assert(exit_code != STILL_ACTIVE);

    if (_state == status_::NOT_READY) return true;
    if (_state == status_::TERMINATING ||
        _state == status_::TERMINATED) {
        return true;
    }

    int status_pre = _state;
    unsigned long result = WAIT_OBJECT_0;

    _ec = exit_code;

    set_status(status_::TERMINATING);
    quit(); // wake up for suspend

    if (status_pre != status_::READY &&
        is_running_internal()) {
        result = WaitForSingleObject(_handle, grace_time);
        if (result == WAIT_TIMEOUT) {
            TerminateThread(_handle, _ec);
        }
    }

    if (_handle) {
        CloseHandle(_handle);
        _handle = NULL;
        _id = 0;
    }

    _owner = NULL;
    _func = NULL;

    set_status(status_::TERMINATED);

    return (result == WAIT_OBJECT_0);
}

template<typename T>
void looper_<T>::quit(void)
{
    _continue = false;

    resume();
}

template<typename T>
void looper_<T>::join(void)
{
    if (is_running_internal()) {
        WaitForSingleObject(_handle, INFINITE);
    }

    destroy(INFINITE);
}

template<typename T>
bool looper_<T>::set_priority(int priority)
{
    _priority = priority;

    return is_valid() ? (SetThreadPriority(_handle, priority) != FALSE) : false;
}

template<typename T>
void looper_<T>::set_exit_code(unsigned long ec)
{
    assert(ec != STILL_ACTIVE);

    _ec = ec;
}

template<typename T>
bool looper_<T>::run(bool fsuspend /*= false*/)
{
    if (_handle == NULL) return false;

    _continue = true;   // loop activation

    set_priority(_priority);
    set_status(status_::STARTING);

    bool ret = ResumeThread(_handle) != (DWORD)(-1);
    if (ret) {
        if (fsuspend) {
            suspend();
        }
    }
    return ret;
}

template<typename T>
void looper_<T>::suspend(void)
{
    for ( ; is_starting(); ) {
        yield();
    }

    if (is_running()) {
        ResetEvent(_suspend);
        set_status(status_::SUSPENDING);
    }
}

template<typename T>
void looper_<T>::resume(void)
{
    if (is_running() != true) {
        SetEvent(_suspend);
    }
}

//////////////////////////////////////////////////////////////////////////

template<typename T>
bool looper_<T>::is_running_internal(void) const
{
    if (_handle == NULL) {
        return false;
    }

    // check thread_activation
    DWORD exitcode = 0;
    GetExitCodeThread(_handle, &exitcode);

    return (exitcode == STILL_ACTIVE);
}

template<typename T>
unsigned __stdcall looper_<T>::core_func_loop(void* param)
{
    this_type* looper = (this_type*)(param);

    if (looper == NULL) {
        assert(false);
        return -1;
    }

    T* owner = looper->_owner;
    FUNC func = looper->_func;

    assert(owner != NULL && func != NULL);

    volatile looper_::status_::TYPE& status = looper->_state;
    const volatile bool& continous = looper->_continue;

    if (status == status_::STARTING) {
        status  = status_::RUNNING;
    }

    // looper core routine //////////////////////

    for ( ; continous; ) {
        if (status == looper_::status_::SUSPENDING) {
            status = looper_::status_::SUSPENDED;
            WaitForSingleObject(looper->handle_suspend(), INFINITE);
            if (continous) {
                status = looper_::status_::RUNNING;
            }
        }
        else {
            (owner->*func)();
        }
    }

    return looper->exit_code();
}

template<typename T>
unsigned __stdcall looper_<T>::core_func_sole(void* param)
{
    this_type* looper = (this_type*)(param);

    if (looper == NULL) {
        assert(false);
        return -1;
    }

    T* owner = looper->_owner;
    FUNC func = looper->_func;

    assert(owner != NULL && func != NULL);

    volatile looper_::status_::TYPE& status = looper->_state;
    const volatile bool& continous = looper->_continue;

    if (status == status_::STARTING) {
        status  = status_::RUNNING;
    }

    if (continous) {
       (owner->*func)();
    }

    return looper->exit_code();
}

//////////////////////////////////////////////////////////////////////////

} // !_namespace_g2

#endif // !_G2_CLIENT_DLL_UTILITY_LOOPER_INL_
