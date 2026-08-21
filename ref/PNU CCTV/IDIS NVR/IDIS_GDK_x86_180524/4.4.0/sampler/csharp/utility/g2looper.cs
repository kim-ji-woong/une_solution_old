using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GDK
{
    public class g2looper
    {
        public enum STATUS
        {
            NOT_READY = 0,
            READY,
            STARTING,
            RUNNING,
            SUSPENDING,
            SUSPENDED,
            TERMINATING,
            TERMINATED
        }

        public g2looper()
        {
            this.thread = null;
            this.runnable = null;
            this.status = STATUS.NOT_READY;
            this.cond_suspend = new EventWaitHandle(false, EventResetMode.ManualReset);
            this.fcontinue = false;
        }
        public bool create(ThreadStart runnable)
        {
            return create(runnable, 0);
        }
        public bool create(ThreadStart runnable, int stack_size)
        {
            if (valid)
            {
                cleanup();
            }

            this.runnable = runnable;
            this.cond_suspend.Reset();
            this.thread = (stack_size != 0) ? new Thread(proc, stack_size) : new Thread(proc);
            this.status = STATUS.READY;
            return valid;
        }
        public void cleanup()
        {
            if (status == STATUS.NOT_READY) return;
            if (status == STATUS.TERMINATED || status == STATUS.TERMINATING) return;

            STATUS status_pre = status;

            status = STATUS.TERMINATING;
            quit();

            if (valid)
            {
                if (status_pre != STATUS.READY &&
                    thread.IsAlive)
                {
                    thread.Join();
                }
            }

            status = STATUS.TERMINATED;
        }
        public bool run(bool fsuspend)
        {
            if (valid != true) return false;
            lock (this)
            {
                fcontinue = true;
                status = STATUS.STARTING;
                thread.Start();
                if (fsuspend)
                {
                    suspend();
                }
            }
            return true;
        }
        public void suspend()
        {
            for (; status == STATUS.STARTING; )
            {
                Thread.Sleep(0);
            }
            if (status == STATUS.RUNNING)
            {
                cond_suspend.Reset();
                status = STATUS.SUSPENDING;
            }
        }
        public void resume()
        {
            if (status == STATUS.RUNNING) return;
            if (status == STATUS.READY)
            {
                run(false);
                return;
            }
            if (fcontinue)
            {

            }
            cond_suspend.Set();
        }
        public void quit()
        {
            fcontinue = false;
            resume();
        }
        public void join()
        {
            if (thread.IsAlive)
            {
                thread.Join();
            }
        }
        static public void sleep(int msec)
        {
            Thread.Sleep(msec);
        }
        static public void spin_wait(int iterations)
        {
            Thread.SpinWait(iterations);
        }

        public bool suspend_for(int msec) { return cond_suspend.WaitOne(msec); }

        public bool is_continue { get { return (fcontinue); } }
        public bool is_ready { get { return (status == STATUS.READY); } }
        public bool is_starting { get { return (status == STATUS.STARTING); } }
        public bool is_running { get { return (status == STATUS.RUNNING); } }
        public bool is_suspended { get { return (status == STATUS.SUSPENDED); } }
        public bool is_suspending { get { return (status == STATUS.SUSPENDING); } }
        public bool is_terminating { get { return (status == STATUS.TERMINATING); } }
        public bool is_terminated { get { return (status == STATUS.TERMINATED); } }
        public bool valid { get { return thread != null; } }

        public string name
        {
            get { return thread != null ? thread.Name : ""; }
            set { if (thread != null) thread.Name = value; }
        }

        protected void proc()
        {
            if (status == STATUS.STARTING)
            {
                status = STATUS.RUNNING;
            }

            if (status == STATUS.RUNNING)
            {
                lock (this) { }
            }

            if (on_begin != null) on_begin(this);

            try
            {
                for (; fcontinue; )
                {
                    if (status == STATUS.SUSPENDING)
                    {
                        status = STATUS.SUSPENDED;
                        suspend_for(G2CONST.GINFINITE);
                        if (fcontinue)
                        {
                            status = STATUS.RUNNING;
                        }
                    }
                    else
                    {
                        if (runnable != null) runnable();
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (ThreadInterruptedException) { }

            if (on_end != null) on_end(this);
        }

        Thread thread;
        ThreadStart runnable;
        STATUS status;
        EventWaitHandle cond_suspend;
        bool fcontinue;

        public delegate void looper_event(g2looper looper);
        public event looper_event on_begin;
        public event looper_event on_end;
    }
}
