// g2looper.h : header file
//

#ifndef _G2_CLIENT_DLL_UTILITY_LOOPER_H_
#define _G2_CLIENT_DLL_UTILITY_LOOPER_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <boost/noncopyable.hpp>

namespace g2 {

//////////////////////////////////////////////////////////////////////////

template<typename T>
class looper_ : private boost::noncopyable
{
public:
    looper_(void);
    ~looper_(void);

    //////////////////////////////////////////////////

    typedef void (G2CALLBACK T::*FUNC)(void);
    typedef looper_<T> this_type;
    typedef bool (this_type::*unspecified_bool_type)() const;

    struct status_ {
        enum TYPE {
            NOT_READY = 0,
            READY,
            STARTING,
            RUNNING,
            SUSPENDING,
            SUSPENDED,
            TERMINATING,
            TERMINATED
        };
    };

protected:
    HANDLE          _handle;
    HANDLE          _suspend;
    unsigned long   _id;
    unsigned long   _ec;
    int             _priority;
    std::wstring    _name;
    T*              _owner;
    FUNC            _func;

protected:
    volatile bool   _continue;
    volatile typename status_::TYPE _state;

public:
    bool create(T* owner, FUNC func, const std::wstring& name, int priority = THREAD_PRIORITY_NORMAL, size_t reserve_stack = 0, bool loop = true);
    bool destroy(unsigned long grace_time = 10000, unsigned long exit_code = 0);
    void quit(void);
    void join(void);

    bool set_priority(int priority);
    void set_exit_code(unsigned long ec);

    bool run(bool suspend);
    void suspend(void);
    void resume(void);

    typename status_::TYPE status(void) const { return _state;  }

    bool is_continue(void)         const { return (_continue);  }
    bool is_ready(void)            const { return (_state == status_::READY);       }
    bool is_starting(void)         const { return (_state == status_::STARTING);    }
    bool is_running(void)          const { return (_state == status_::RUNNING);     }
    bool is_suspended(void)        const { return (_state == status_::SUSPENDED);   }
    bool is_suspending(void)       const { return (_state == status_::SUSPENDING);  }
    bool is_terminating(void)      const { return (_state == status_::TERMINATING); }
    bool is_terminated(void)       const { return (_state == status_::TERMINATED);  }
    bool is_valid(void)            const { return (safe_handle() != NULL);          }

    int  priority(void)            const { return _priority;    }
    unsigned long threadId(void)   const { return _id;          }
    const std::wstring& name(void) const { return _name;        }
    HANDLE safe_handle(void)       const { return (this != NULL) ? _handle : NULL; }

    static bool yield(void)              { return (::SwitchToThread() == TRUE); }
    static void yield_priority(void)     { ::Sleep(0);          }
    static void sleep(DWORD msec)        { ::Sleep(msec);       }

protected:
    bool is_running_internal(void) const;
    unsigned long exit_code(void)  const { return _ec;          }
    HANDLE handle_suspend(void)    const { return _suspend;     }
    void set_status(typename status_::TYPE status) { _state = status; }

    static unsigned __stdcall core_func_loop(void* param);
    static unsigned __stdcall core_func_sole(void* param);

public:
    operator bool()  const { return is_valid(); }
    bool operator!() const { return is_valid() != true; }
    operator unspecified_bool_type() const { return (is_valid()) ? &this_type::is_valid : NULL; }
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_g2

#include <utility/g2looper.inl>

#endif // !_G2_CLIENT_DLL_UTILITY_LOOPER_H_
