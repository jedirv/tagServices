from flask import Flask
from flask import jsonify
import sqlite3
from flask import g

DATABASE = 'tagBackend.db'

app = Flask(__name__)

@app.route('/')
def hello_world():
    return 'index page'
 
  
@app.route('/tagapi/tagsForEmail/<entryID>')
def get_tags_for_email(entryID):
    query = "SELECT Email.ID from Email WHERE Email.EntryID='{0}';".format(entryID)
    print(query)
    all_rows = query_db(query)
    row = all_rows[0]
    emailID = row['ID']
    print("emailID was {0}".format(emailID))
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
   

    
@app.route('/tagapi/foo/<treename>')
def show_user_profile(treename):
    # show the user profile for that user
    return 'here is tree %s' % treename

@app.route('/tagapi/docs/<tag>')
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
 
    