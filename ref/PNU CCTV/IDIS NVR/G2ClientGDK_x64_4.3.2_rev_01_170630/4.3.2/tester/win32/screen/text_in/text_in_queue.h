// text_in_queue.h : header file
//

#ifndef _TEXT_IN_QUEUE_H_
#define _TEXT_IN_QUEUE_H_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <include/g2_define.h>
#include <utility/g2lock.h>
#include <boost/shared_ptr.hpp>
#include <boost/noncopyable.hpp>

namespace client {

//////////////////////////////////////////////////////////////////////////

class text_in_queue : private boost::noncopyable
{
public:
    text_in_queue(void);
    ~text_in_queue(void);

    typedef boost::shared_ptr<G2TEXT_IN_ELEMENT> element_sp;
    typedef CList<element_sp> container_type;

protected:
    int _from;

    struct scope_t {
        G2TIME  _first;
        G2TIME  _last;
    }
    _scope;

    container_type _list;

private:
    g2::critical_section _lock;

public:
    void put(const G2TEXT_IN_ELEMENT& data, bool tohead);
    void push_front(element_sp element);
    void push_back(element_sp element);

    element_sp pop_back(void);
    element_sp pop_front(void);
    element_sp at_prev(POSITION& pos);
    element_sp at_next(POSITION& pos);

    void clear_recent(void);
    void clear(void);

    const G2TIME& scope_first(void) const { return _scope._first; }
    const G2TIME& scope_last(void) const { return _scope._last; }

    bool empty(void) const { return (_list.IsEmpty() == TRUE); }
    int size(void) const { return static_cast<int>(_list.GetCount()); }
    int from(void) const { return _from; }

    void find_text_in_range(POSITION& from, POSITION& to, const G2SPOT& spot);

private:
    bool equal(const G2TEXT_IN_ELEMENT& left, const G2TEXT_IN_ELEMENT& right);

public:
    bool try_lock(void) const { return _lock.trylock(); }
    void lock(void) const { _lock.lock(); }
    void unlock(void) const { _lock.unlock(); }
    void enter(void) const { _lock.enter(); }
    void leave(void) const { _lock.leave(); }
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif  // !_TEXT_IN_QUEUE_H_
