
import sqlitedb as db
from flask import jsonify
import util
   
def is_tag_in_db(tag):
    tagID = get_tag_id_for_tag(tag)
    if (tagID == '?'):
	    return False
    return True

def get_tag_id_for_tag(tag):
    query = "SELECT Tags.ID from Tags WHERE Tags.Name='{0}';".format(tag)
    all_rows = db.query_db(query)
    return util.get_id_from_rows(all_rows)

def add_tag(tag):
    query = "INSERT INTO Tags (Name) VALUES ('{0}');".format(tag)
    print(query)
    return db.insert_and_get_id(query, 'Tags')

def get_all_tags():
    query = "SELECT Tags.Name from Tags"
    tagList = []
    for tag_row in db.query_db(query):
        tag = tag_row['Name']
        tagDict = { "Name" : tag }
        tagList.append(tagDict)
    
    result = {}
    result["Tags"] = tagList
    response = jsonify(result)
    return response