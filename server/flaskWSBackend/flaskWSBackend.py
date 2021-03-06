#FLASK_APP=flaskWSBackend.py
#FLASK_DEBUG=1

from flask import Flask, request

import tagTable
import resourceTable
import emailTable
import personTable
import resourceTagTable
import emailTagTable
import personTagTable
import tester
import util
import sqlitedb as db
import os


app = Flask(__name__)
@app.route('/')
def hello_world():
    return 'index page'
 

@app.route('/tagapi/addTag/', methods=['GET'])
def add_tag():
    tag = request.args.get('tag')
    if tagTable.is_tag_in_db(tag):
        return util.insert_rejected('tag {0} already exists'.format(tag))
    else:
        return tagTable.add_tag(tag)
        
@app.route('/tagapi/addResource/', methods=['GET'])
def add_resource():
    resource_type = request.args.get('resourceType')
    name = request.args.get('name')
    if resourceTable.is_resource_in_db(resource_type, name):
        return util.insert_rejected('resource {0} of type {1} already exists'.format(name, resource_type))
    else:
        return resourceTable.add_resource(resource_type, name)
        
        
@app.route('/tagapi/addPerson/', methods=['GET'])
def add_person():
    name = request.args.get('name')
    if personTable.is_person_in_db(name):
        return util.insert_rejected('person {0} already exists'.format(name))
    else:
        return personTable.add_person(name)
    
@app.route('/tagapi/showPersons')
def show_persons():
    return personTable.show_persons()
        

@app.route('/tagapi/addEmail/', methods=['GET'])
def add_email():
    conversationID = request.args.get('conversationID')
    entryID = request.args.get('entryID')
    if emailTable.is_email_in_db(conversationID, entryID):
        return util.insert_rejected('emails entry {0} already exists'.format(entryID))
    else:
        return emailTable.add_email(conversationID, entryID)
        

@app.route('/tagapi/tagPerson/', methods=['GET'])
def tag_person():
    tag = request.args.get('tag')
    person = request.args.get('name')
    if personTagTable.is_person_tag_in_db(person, tag):
        return util.insert_rejected('person {0} already tagged with {1}'.format(person, tag))
    else:
        return personTagTable.add_person_tag(person, tag)

@app.route('/tagapi/tagEmail/', methods=['GET'])
def tag_email():
    entryID = request.args.get('entryID')
    tag = request.args.get('tag')
    if emailTagTable.is_email_tag_in_db(entryID, tag):
        return util.insert_rejected('email {0} already tagged with {1}'.format(entryID, tag))
    else:
        return emailTagTable.add_email_tag(entryID, tag)
        
@app.route('/tagapi/untagEmail/', methods=['GET'])
def untag_email():
    entryID = request.args.get('entryID')
    tag = request.args.get('tag')
    if not(emailTagTable.is_email_tag_in_db(entryID, tag)):
        return util.delete_rejected('email {0} was not tagged with {1}'.format(entryID, tag))
    else:
        return emailTagTable.delete_email_tag(entryID, tag)
        


@app.route('/tagapi/tagResource/', methods=['GET'])
def tag_resource():
    type = request.args.get('type')
    name = request.args.get('name')
    tag = request.args.get('tag')
    if resourceTagTable.is_resource_tag_in_db(type, name, tag):
        return util.insert_rejected('resource {0} {1} already tagged with {2}'.format(type, name, tag))
    else:
        return resourceTagTable.add_resource_tag(type, name, tag)
        

        
@app.route('/tagapi/allTags')
def get_all_tags():
    return tagTable.get_all_tags()
   
@app.route('/tagapi/tagsForEmail/', methods=['GET'])
def get_tags_for_email():
    entryID = request.args.get('entryID')
    return emailTagTable.get_tags_for_email(entryID)


@app.route('/tagapi/docsForTag/', methods=['GET'])
def get_docs_for_tag():
    tag = request.args.get('tag')
    return resourceTagTable.get_docs_for_tag(tag)
    

@app.route('/tagapi/testing')
def show_user_profile(treename):
    # show the user profile for that user
    return 'flask web services up'

@app.route('/tagapi/dummydocs/<tag>')
def get_dummy_document_tree(tag):
    return resourceTagTable.get_dummy_document_tree
    
@app.route('/tagapi/runtests')
def run_tests():
    return tester.run_tests()
    
@app.teardown_appcontext
def close_connection(exception):
    db.close_connection(exception)

#def unescape_url_param(s):
#    s = s.replace("%5C", "\\")
#    return s.replace("+"," ")
  
if __name__ == '__main__':
    os.chdir("C:\\Users\\sudo\\tagServices\\server\\flaskWSBackend")
    # http://librelist.com/browser/flask/2011/5/12/using-eclipse+pydev-for-debugging-flask-apps/
    app.debug = False
    app.run()
    
