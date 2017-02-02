
import sqlitedb as db
import tag
import person

def is_person_tag_in_db(person, tag):
    personTagID = get_person_tag_id(person, tag)
    if (personTagID=='?'):
        return False
    return True

def get_person_tag_ID(person, tag):
    personID = person.get_person_id_for_person(person)
    tagID = tag.get_tag_id_for_tag(tag)
    if (personID=='?' or tagID=='?'):
        return '?'
    query="SELECT PersonTags.ID FROM PersonTags WHERE PersonID='{0}' and TagID='{1}'".format(personID, tagID)
    personTagID = '?'
    all_rows = db.query_db(query)
    if (all_rows):
        row = all_rows[0]
        personTagID = row['ID']
    return personTagID

def add_person_tag(person, tag):
    personID = person.get_person_id_for_person(person)
    tagID = tag.get_tag_id_for_tag(tag)
    query = "INSERT INTO PersonTags (PersonID, TagID) VALUES ('{0}','{1}');".format(personID, tagID)
    return db.insert_and_get_id(query, 'PersonTags')