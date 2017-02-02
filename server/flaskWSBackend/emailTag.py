
import sqlitedb as db


def get_tags_for_email(entryID):
    query = "SELECT Email.ID from Email WHERE Email.EntryID='{0}';".format(entryID)
    all_rows = db.query_db(query)
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