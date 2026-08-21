// text_in_queue.cpp : implementation file
//

#include "stdafx.h"
#include "text_in_queue.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

text_in_queue::text_in_queue(void)
    : _from(G2TEXT_IN_ELEMENT::FROM_LIVE)
{

}

text_in_queue::~text_in_queue(void)
{

}

//////////////////////////////////////////////////////////////////////////

void text_in_queue::put(const G2TEXT_IN_ELEMENT& data, bool tohead)
{
    g2::scoped_criticalsection lock(_lock);

    _from = data._from;
    _scope._last = data._spot._time;

    if (_from == G2TEXT_IN_ELEMENT::FROM_PLAY) {
        bool repeat = false;
        element_sp element;
        for (POSITION pos = _list.GetTailPosition(); pos != NULL; ) {
            if (element = _list.GetPrev(pos))
            if (equal(data, *element)) {
                repeat = true;
                break;
            }
        }
        if (repeat) {
            TRACE("$$ text_in_queue:: exist text-in repetition\n");
            return;
        }
    }

    if (tohead) {
        if (size() >= 300) {
            _list.RemoveTail();
        }

        element_sp element = element_sp(new G2TEXT_IN_ELEMENT);
        memcpy(element.get(), &data, sizeof(data));
        _list.AddHead(element);
    }
    else {
        // queue reserved 300 elements,
        // push at tail in case of general.
        if (size() >= 300) {
            _list.RemoveHead();
        }
        element_sp element = element_sp(new G2TEXT_IN_ELEMENT);
        memcpy(element.get(), &data, sizeof(data));
        _list.AddTail(element);
    }
}

void text_in_queue::push_front(element_sp element)
{
    g2::scoped_criticalsection lock(_lock);

    _list.AddHead(element);
}

void text_in_queue::push_back(element_sp element)
{
    g2::scoped_criticalsection lock(_lock);

    _list.AddTail(element);
}

text_in_queue::element_sp text_in_queue::pop_back(void)
{
    g2::scoped_criticalsection lock(_lock);

    return _list.RemoveTail();
}

text_in_queue::element_sp text_in_queue::pop_front(void)
{
    g2::scoped_criticalsection lock(_lock);

    return _list.RemoveHead();
}

text_in_queue::element_sp text_in_queue::at_prev(POSITION& pos)
{
    g2::scoped_criticalsection lock(_lock);

    return _list.GetPrev(pos);
}

text_in_queue::element_sp text_in_queue::at_next(POSITION& pos)
{
    g2::scoped_criticalsection lock(_lock);

    if (pos == NULL) pos = _list.GetHeadPosition();
    return (pos != NULL) ? _list.GetNext(pos) : element_sp();
}

void text_in_queue::clear_recent(void)
{
    // if may judge that don't need draw when display text-in
    // delete one bunch of the most recent text-in.

    g2::scoped_criticalsection lock(_lock);

    element_sp element;
    for (int i = 0, count = size(); i < count; ++i) {
        if (element = _list.RemoveHead())
        if (element->_type == G2TEXT_IN_ELEMENT::TYPE_END) {
            break;
        }
    }

    if (size() > 0) {
        if (element = _list.GetHead()) {
            _scope._first = element->_spot._time;
        }
    }
    else {
        g2_time_make_invalid(&_scope._first);
    }
}

void text_in_queue::clear(void)
{
    g2::scoped_criticalsection lock(_lock);

    _list.RemoveAll();
    g2_time_make_invalid(&_scope._first);
}

void text_in_queue::find_text_in_range(POSITION& from, POSITION& to, const G2SPOT& spot)
{
    g2::scoped_criticalsection lock(_lock);

    from = to = NULL;

    POSITION pos = NULL;
    element_sp element;

    CTime time = CTime(g2_time_to_time32_t(&spot._time)) - CTimeSpan(0, 0, 0, 10);
    G2TIME limit;
    g2_time_from_time32_t(&limit, (unsigned long)time.GetTime());

    // seek data to before 10 seconds at time-line
    for (pos = _list.GetTailPosition(); pos != NULL; ) {
        if (element = _list.GetPrev(pos))
        if (g2_spot_compare(&spot, &element->_spot) < 0) continue;
        if (g2_time_compare(&limit, &element->_spot._time) < 0) {
            if (pos == NULL) {
                to = _list.GetTailPosition();
            }
            else {
                _list.GetNext(pos);
                to = pos;
            }
            break;
        }
    }

    if (to == NULL) return;

    for (pos = to; pos != NULL; ) {
        from = pos;
        if (element = _list.GetPrev(pos))
        if (g2_time_compare(&element->_spot._time, &limit) <= 0) {
            break;
        }
    }
}

bool text_in_queue::equal(const G2TEXT_IN_ELEMENT& left, const G2TEXT_IN_ELEMENT& right) 
{
    return (left._camera == right._camera &&
        left._type == right._type &&
        left._from == right._from &&
        g2_spot_compare(&left._spot, &right._spot) == 0 &&
        wcscmp(left._data._string, right._data._string) == 0);
}
