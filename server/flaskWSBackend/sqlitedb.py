import sqlite3
from flask import g
import util

DATABASE = 'tagBackend.db'


def get_db():
    db = getattr(g, '_database', None)
    if db is None:
        db = g._database = sqlite3.connect(DATABASE)
        db.row_factory = sqlite3.Row
    return db
	

def query_db(query, args=()):
    cur = get_db().execute(query, args)
    rv = cur.fetchall()
    cur.close()
    return rv

def change_db(query, args=()):
    cur = get_db().execute(query, args)
    get_db().commit()
    rv = cur.fetchall()
    cur.close()
    return rv


def insert_and_get_id(insert_command, table):
    print(insert_command)
    all_rows = change_db(insert_command)
    query = "SELECT max(id) from {0};".format(table)
    all_rows = query_db(query)
    return util.insert_succeeded(all_rows[0][0])

def delete(delete_command, id):
    print(delete_command)
    all_rows = change_db(delete_command)
    return util.delete_succeeded(id)

def close_connection(exception):
    db = getattr(g, '_database', None)
    if db is not None:
        db.close()	