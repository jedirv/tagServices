
import sqlitedb as db
import tagTable
import personTable
import util

def is_person_tag_in_db(person, tag):
    print("person is {0}, tag is {1}".format(person, tag))
    personTagID = get_person_tag_id(person, tag)
    if (personTagID=='?'):
        return False
    return True

def get_person_tag_id(person, tag):
    print("person is {0}, tag is {1}".format(person, tag))
    personID = personTable.get_person_id_for_person(person)
    tagID = tagTable.get_tag_id_for_tag(tag)
    if (personID=='?' or tagID=='?'):
        return '?'
    query="SELECT PersonTags.ID FROM PersonTags WHERE PersonID='{0}' and TagID='{1}'".format(personID, tagID)
    
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)

def add_person_tag(person, tag):
    personID = personTable.get_person_id_for_person(person)
    print('personID is {0}'.format(personID))
    tagID = tagTable.get_tag_id_for_tag(tag)
    if (tagID=='?'):
        return util.insert_rejected('tag {0} does not exist'.format(tag))
    if (personID=='?'):
        return util.insert_rejected('person {0} does not exist'.format(person))
    print('tagID is {0}'.format(tagID))
    query = "INSERT INTO PersonTags (PersonID, TagID) VALUES ('{0}','{1}');".format(personID, tagID)
    return db.insert_and_get_id(query, 'PersonTags')