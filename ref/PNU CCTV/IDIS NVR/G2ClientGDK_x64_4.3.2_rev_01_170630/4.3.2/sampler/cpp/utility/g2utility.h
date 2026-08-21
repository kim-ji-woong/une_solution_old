// g2utility.h : header file
//

#ifndef _G2_CLIENT_DLL_UTILITY_H_
#define _G2_CLIENT_DLL_UTILITY_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

namespace g2 {

//////////////////////////////////////////////////////////////////////////

#ifndef SAFE_DELETE
#define SAFE_DELETE(LP)     { if (LP) { delete (LP);     (LP) = NULL; } }
#endif
#ifndef SAFE_DELETE_ARR
#define SAFE_DELETE_ARR(LP) { if (LP) { delete [] (LP);  (LP) = NULL; } }
#endif

//////////////////////////////////////////////////////////////////////////

#ifndef _countof
template <typename T, size_t N>
char (*__array_size_helper(UNALIGNED T (&_array)[N]))[N];
#define _countof(_array) sizeof(*g2::__array_size_helper(_array))
#endif

//////////////////////////////////////////////////////////////////////////

#ifndef G2RETURN_IF_FAIL
#define G2RETURN_IF_FAIL(expr) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed.\n", \
            __FILE__,   \
            __LINE__,   \
            #expr);     \
        return;         \
    }                   }
#endif

#ifndef G2RETURN_VAL_IF_FAIL
#define G2RETURN_VAL_IF_FAIL(expr, val) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed.\n", \
            __FILE__,   \
            __LINE__,   \
            #expr);     \
        return (val);   \
    }                   }
#endif

#ifndef G2RETURN_VAL_IF_FAIL_POST
#define G2RETURN_VAL_IF_FAIL_POST(expr, val, post) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed.\n", \
            __FILE__,   \
            __LINE__,   \
            #expr);     \
        (post);         \
        return (val);   \
    }                   }
#endif

#ifndef G2RETURN_IF_ASSERT
#define G2RETURN_IF_ASSERT(expr) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed.\n", \
            __FILE__,   \
            __LINE__,   \
            #expr);     \
        assert((expr)); \
        return;         \
    }                   }
#endif

#ifndef G2RETURN_VAL_IF_ASSERT
#define G2RETURN_VAL_IF_ASSERT(expr, val) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed.\n", \
            __FILE__,   \
            __LINE__,   \
            #expr);     \
        assert((expr)); \
        return (val);   \
    }                   }
#endif

#ifndef G2RETURN_IF_ASSERTEX
#define G2RETURN_IF_ASSERTEX(expr, reason) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed. reason: %s\n", \
            __FILE__,   \
            __LINE__,   \
            #expr,      \
            reason);    \
        assert((expr)); \
        return;         \
    }                   }
#endif

#ifndef G2RETURN_VAL_IF_ASSERTEX
#define G2RETURN_VAL_IF_ASSERTEX(expr, val, reason) { \
    if (!(expr)) {      \
        TRACE("%s(%d): assertion '%s' failed. reason: %s\n", \
            __FILE__,   \
            __LINE__,   \
            #expr,      \
            reason);    \
        assert((expr) && !(reason)); \
        return (val);   \
    }                   }
#endif

//////////////////////////////////////////////////////////////////////////

} // !_namespage_g2

#endif // !_G2_CLIENT_DLL_UTILITY_H_
