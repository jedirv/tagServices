#FLASK_APP=flaskWSBackend.py
#FLASK_DEBUG=1

from flask import Flask
from flask import jsonify
import sqlite3
from flask import g

DATABASE = 'tagBackend.db'

app = Flask(__name__)

@app.route('/')
def hello_world():
    return 'index page'
 

@app.route('/tagapi/addTag/<tag>')
def add_tag(tag):
	all_rows = get_tag_ID_for_tagname(tag)
	if (all_rows):
	    return insert_rejected('tag {0} already exists'.format(tag))
	else:
		query = "INSERT INTO Tags (Name) VALUES ('{0}');".format(tag)
		print(query)
		return insert_and_get_id(query, 'Tags')

def insert_and_get_id(insert_command, table):
    print(insert_command)
    all_rows = insert_db(insert_command)
    query = "SELECT max(id) from {0};".format(table)
    all_rows = query_db(query)
    print(all_rows[0][0])
    result = {}
    result["LastInsertID"] = all_rows[0][0]
    response = jsonify(result)
    return response

@app.route('/tagapi/addEmail/<conversationID>/<entryID>')
def add_email(conversationID, entryID):
    all_rows = get_email_ID_for_these(conversationID, entryID)
    if (all_rows):
        return insert_rejected('emails entry {0} already exists'.format(entryID))
    else:
        query = "INSERT INTO Emails (EntryID, ConversationID) VALUES ('{0}','{1}');".format(entryID, conversationID)
        return insert_and_get_id(query, 'Emails')
  
@app.route('/tagapi/tagsForEmail/<entryID>')
def get_tags_for_email(entryID):
    query = "SELECT Email.ID from Email WHERE Email.EntryID='{0}';".format(entryID)
    all_rows = query_db(query)
    row = all_rows[0]
    emailID = row['ID']
    query = "SELECT Tags.Name FROM Tags INNER Join EmailTags on EmailTags.tagID=Tags.ID WHERE EmailID='{0}';".format(emailID)
    tagList = []
    for tag_row in query_db(query):
        tag = tag_row['Name']
        tagDict = { "Name" : tag }
        tagList.append(tagDict)
    
    result = {}
    result["Tags"] = tagList
    response = jsonify(result)
    return response
    #'{\"Tags\":[{\"Name\":\"tag1\"},{\"Name\":\"tag2\"},{\"Name\":\"tag3\"}]}'

    
def get_tag_ID_for_tagname(tagName):
    query = "SELECT Tags.ID from Tags WHERE Tags.Name='{0}';".format(tagName)
    all_rows = query_db(query)
    return all_rows
	

def get_email_ID_for_these(conversationID, entryID):
    query = "SELECT Emails.ID from Emails WHERE Emails.ConversationID='{0}' and Emails.EntryID='{1}';".format(conversationID, entryID)
    all_rows = query_db(query)
    return all_rows

def get_doc_tree_for_tag_id(tagID):
    print("tag was {0}".format(tagID))
    mru_query = "SELECT Resources.Name FROM Resources INNER JOIN ResourceTags ON Resources.ID=ResourceTags.ResourceID WHERE Resources.Type='FILE' AND ResourceTags.TagID='{0}' ORDER BY Resources.LastUse DESC LIMIT 2;".format(tagID)
    mruDocList = []
    for doc_row in query_db(mru_query):
        doc = doc_row['Name']
        docDict = { "Name" : doc }
        mruDocList.append(docDict)
    
    rel_query = "SELECT Resources.Name FROM Resources INNER JOIN ResourceTags ON Resources.ID=ResourceTags.ResourceID WHERE Resources.Type='FILE' AND ResourceTags.TagID='{0}' ORDER BY Resources.Name;".format(tagID)
    relDocList = []
    for doc_row in query_db(rel_query):
        doc = doc_row['Name']
        docDict = { "Name" : doc }
        relDocList.append(docDict)
    result = {}
    result["RelevantDocuments"] = relDocList
    result["MruDocuments"] =  mruDocList
    response = jsonify(result);
    return response

    
def get_empty_doc_tree():
    mruDocList = []
    relDocList = []
    result = {}
    result["RelevantDocuments"] = relDocList
    result["MruDocuments"] =  mruDocList
    response = jsonify(result);
    return response
    
@app.route('/tagapi/docsForTag/<tagName>')
def get_docs_for_tag(tagName):
    all_rows = get_tag_ID_for_tagname(tagName)
    print(all_rows)
    if (all_rows):
        row = all_rows[0]
        tagID = row['ID']
        return get_doc_tree_for_tag_id(tagID)
    else:
        return get_empty_doc_tree()
    print(all_rows)
    
    

    
@app.route('/tagapi/foo/<treename>')
def show_user_profile(treename):
    # show the user profile for that user
    return 'here is tree %s' % treename

@app.route('/tagapi/dummydocs/<tag>')
def get_document_tree(tag):
    #'{\"RelevantDocuments\":[{\"Name\":\"docA1\"},{\"Name\":\"docA2\"},{\"Name\":\"docA3\"}],"MruDocuments":[{\"Name\":\"docA4\"},{\"Name\":\"docA5\"},{\"Name\":\"docA6\"}]}'
    doc1Dict = { "Name" : "docA1" }
    doc2Dict = { "Name" : "docA2" }
    doc3Dict = { "Name" : "docA3" }
    relDocList = []
    relDocList.append(doc1Dict);
    relDocList.append(doc2Dict);
    relDocList.append(doc3Dict);
    
    doc4Dict = { "Name" : "docA4" }
    doc5Dict = { "Name" : "docA5" }
    doc6Dict = { "Name" : "docB6" }
    mruDocList = []
    mruDocList.append(doc4Dict);
    mruDocList.append(doc5Dict);
    mruDocList.append(doc6Dict);
    
    result = {}
    result["RelevantDocuments"] = relDocList
    result["MruDocuments"] =  mruDocList
    response = jsonify(result);
    return response
    
    
def get_db():
    db = getattr(g, '_database', None)
    if db is None:
        db = g._database = sqlite3.connect(DATABASE)
        db.row_factory = sqlite3.Row
    return db

@app.teardown_appcontext
def close_connection(exception):
    db = getattr(g, '_database', None)
    if db is not None:
        db.close()
        
def query_db(query, args=()):
    cur = get_db().execute(query, args)
    rv = cur.fetchall()
    cur.close()
    return rv

def insert_db(query, args=()):
    cur = get_db().execute(query, args)
    get_db().commit()
    rv = cur.fetchall()
    cur.close()
    return rv
 
    