# -*- coding: utf-8 -*-
#!/usr/bin/env python

import sqlite3
import sys
from sql_requests import *


def get_history(date):
    conn, cursor = _get_connection()
    data = cursor.execute(SELECT_FROM_HISTORY_BY_GTE_DATE, [(str(date))]).fetchall()
    return data


get_history(sys.args[1])
