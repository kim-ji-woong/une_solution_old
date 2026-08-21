// listener_search_g2_event_list.h : header file
//

#ifndef _LISTENER_SEARCH_G2_EVENT_LIST_H_
#define _LISTENER_SEARCH_G2_EVENT_LIST_H_

namespace client {

//////////////////////////////////////////////////////////////////////////

class listener_search_g2_event_list
{
protected:
    listener_search_g2_event_list(void) {
    }

public:
    virtual ~listener_search_g2_event_list(void) {
    }

protected:
    virtual void on_request_more_event_search_g2(void) = 0;
    virtual void on_request_load_event_image_search_g2(G2SPOT spot) = 0;

private:
    friend class search_g2_event_list;
};

//////////////////////////////////////////////////////////////////////////

} // !_namespace_client

#endif // !_LISTENER_SEARCH_G2_EVENT_LIST_H_