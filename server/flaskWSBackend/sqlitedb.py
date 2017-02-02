import sqlite3


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

def insert_db(query, args=()):
    cur = get_db().execute(query, args)
    get_db().commit()
    rv = cur.fetchall()
    cur.close()
    return rv
	

def insert_and_get_id(insert_command, table):
    print(insert_command)
    all_rows = insert_db(insert_command)
    query = "SELECT max(id) from {0};".format(table)
    all_rows = query_db(query)
    return insert_succeeded(all_rows[0][0])

	