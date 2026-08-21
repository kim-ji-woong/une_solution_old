// listener_search_event_list.h : header file
//

#ifndef _LISTENER_SEARCH_EVENT_LIST_H_
#define _LISTENER_SEARCH_EVENT_LIST_H_

namespace client {

//////////////////////////////////////////////////////////////////////////

class listener_search_event_list
{
protected:
    listener_search_event_list(void) {
    }

public:
    virtual ~listener_search_event_list(void) {
    }

protected:
    virtual void on_request_more_event_search(void) = 0;
    virtual void on_request_load_event_image_search(int selected, bool last) = 0;

private:
    friend class search_event_list;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_LISTENER_SEARCH_EVENT_LIST_H_