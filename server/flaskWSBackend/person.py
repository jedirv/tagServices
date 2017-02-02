import sqlitedb as db


def is_person_in_db(person):
    all_rows = get_person_ID_for_personName(person)
    if (all_rows):
        return True
    return False
	
def get_person_ID_for_personName(personName):
    query = "SELECT Persons.ID from Persons WHERE Persons.Name='{0}';".format(personName)
    all_rows = db.query_db(query)
    return all_rows	
	
def add_person(person):
    query = "INSERT INTO Persons (Name) VALUES ('{0}');".format(person)
    return db.insert_and_get_id(query, 'Persons')
	