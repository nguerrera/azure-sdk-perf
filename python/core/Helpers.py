import random
import string
from threading import Timer


def NewGuid(length=10):
    return ''.join(random.choice(string.ascii_lowercase + string.digits) for _ in range(length))


# Credit to https://stackoverflow.com/questions/3393612/run-certain-code-every-n-seconds
class RepeatedTimer(object):
    def __init__(self, interval, function, *args, **kwargs):
        self._timer     = None
        self.interval   = interval
        self.function   = function
        self.args       = args
        self.kwargs     = kwargs
        self.is_running = False
        self.start()


    def _run(self):
        self.is_running = False
        self.start()
        self.function(*self.args, **self.kwargs)


    def start(self):
        if not self.is_running:
            #NOTE: If there is a concern about perf impact of this Timer, we'd need to convert to multiprocess and use IPC.

            self._timer = Timer(self.interval, self._run)
            self._timer.start()
            self.is_running = True


    def stop(self):
        self._timer.cancel()
        self.is_running = False