import sqlitedb as db
import util

def is_person_in_db(person):
    print("person is {0}".format(person))
    all_rows = get_person_id_for_person(person)
    if (all_rows):
        return True
    return False
	
def get_person_id_for_person(person):
    query = "SELECT Persons.ID from Persons WHERE Persons.Name='{0}';".format(person)
    print(query)
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)
	
def add_person(person):
    query = "INSERT INTO Persons (Name) VALUES ('{0}');".format(person)
    return db.insert_and_get_id(query, 'Persons')
	