
import sqlitedb as db

def is_resource_in_db(type, name):
    all_rows = get_resource_ID_for_these(type, name)
    if (all_rows):
        return True
    return False
	
def get_resource_ID_for_these(type, name):
    query = "SELECT Resources.ID from Resources WHERE Resources.Type='{0}' and Resources.Name='{1}';".format(type, name)
    all_rows = db.query_db(query)
    return all_rows

def add_resource(type, name):
    query = "INSERT INTO Resources (Type, Name) VALUES ('{0}','{1}');".format(type, name)
    return db.insert_and_get_id(query, 'Resources')

    