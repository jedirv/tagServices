
import sqlitedb as db
   
def is_tag_in_db(tag):
    tagID = tag.get_tag_ID_for_tagname(tag)
    if (tagID == '?'):
	    return False
    return True

def get_tag_ID_for_tagname(tagName):
    query = "SELECT Tags.ID from Tags WHERE Tags.Name='{0}';".format(tagName)
    all_rows = db.query_db(query)
    tagID = '?'
    if (all_rows):
        row = all_rows[0]
        tagID = row['ID']
    return tagID

def add_tag(tag):
    query = "INSERT INTO Tags (Name) VALUES ('{0}');".format(tag)
    print(query)
    return db.insert_and_get_id(query, 'Tags')