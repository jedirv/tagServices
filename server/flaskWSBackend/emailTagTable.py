
import sqlitedb as db
from flask import jsonify
import emailTable
import tagTable
import util

def is_email_tag_in_db(entryID, tag):
    print("email is {0}, tag is {1}".format(entryID, tag))
    emailTagID = get_email_tag_id(entryID, tag)
    if (emailTagID=='?'):
        return False
    return True

def get_email_tag_id(entryID, tag):
    print("person is {0}, tag is {1}".format(entryID, tag))
    emailID = emailTable.get_email_id_for_email(entryID)
    tagID = tagTable.get_tag_id_for_tag(tag)
    if (emailID=='?' or tagID=='?'):
        return '?'
    query="SELECT EmailTags.ID FROM EmailTags WHERE EmailID='{0}' and TagID='{1}'".format(emailID, tagID)
    
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)

def add_email_tag(entryID, tag):
    emailID = emailTable.get_email_id_for_email(entryID)
    print('emailID is {0}'.format(emailID))
    tagID = tagTable.get_tag_id_for_tag(tag)
    if (tagID=='?'):
        return util.insert_rejected('tag {0} does not exist'.format(tag))
    if (emailID=='?'):
        return util.insert_rejected('email {0} does not exist'.format(entryID))
    print('tagID is {0}'.format(tagID))
    query = "INSERT INTO EmailTags (EmailID, TagID) VALUES ('{0}','{1}');".format(emailID, tagID)
    return db.insert_and_get_id(query, 'PersonTags')


def get_tags_for_email(entryID):
    query = "SELECT Emails.ID from Emails WHERE Emails.EntryID='{0}';".format(entryID)
    all_rows = db.query_db(query)
    if not(all_rows):
        return get_empty_tags_response()
    row = all_rows[0]
    emailID = row['ID']
    query = "SELECT Tags.Name FROM Tags INNER Join EmailTags on EmailTags.tagID=Tags.ID WHERE EmailID='{0}';".format(emailID)
    tagList = []
    for tag_row in db.query_db(query):
        tag = tag_row['Name']
        tagDict = { "Name" : tag }
        tagList.append(tagDict)
    
    result = {}
    result["Tags"] = tagList
    response = jsonify(result)
    return response
    #'{\"Tags\":[{\"Name\":\"tag1\"},{\"Name\":\"tag2\"},{\"Name\":\"tag3\"}]}'
    
def get_empty_tags_response():
    result = {}
    result["Tags"] = []
    response = jsonify(result)
    return response