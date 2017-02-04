import sqlitedb as db
import util

def is_person_in_db(person):
    print("person is {0}".format(person))
    id = get_person_id_for_person(person)
    if (id == '?'):
        return False
    return True
	
def get_person_id_for_person(person):
    query = "SELECT Persons.ID from Persons WHERE Persons.Name='{0}';".format(person)
    print(query)
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)
	
def add_person(person):
    query = "INSERT INTO Persons (Name) VALUES ('{0}');".format(person)
    return db.insert_and_get_id(query, 'Persons')

def show_persons():
    query = "SELECT Persons.Name FROM Persons"
    all_rows = db.query_db(query)
    list_string = ''
    for row in all_rows:
        list_string = list_string + "\n" + row['Name']
    return util.get_insert_response("debugQuery Persons", list_string);
	