#FLASK_APP=flaskWSBackend.py
#FLASK_DEBUG=1

from flask import Flask

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


app = Flask(__name__)
@app.route('/')
def hello_world():
    return 'index page'
 

@app.route('/tagapi/addTag/<tag>')
def add_tag(tag):
	if tagTable.is_tag_in_db(tag):
	    return util.insert_rejected('tag {0} already exists'.format(tag))
	else:
	    return tagTable.add_tag(tag)
		
@app.route('/tagapi/addResource/<type>/<name>')
def add_resource(type, name):
    if resourceTable.is_resource_in_db(type, name):
        return util.insert_rejected('resource {0} of type {1} already exists'.format(name, type))
    else:
	    return resourceTable.add_resource(type, name)
        
		
@app.route('/tagapi/addPerson/<person>')
def add_person(person):
    if personTable.is_person_in_db(person):
        return util.insert_rejected('person {0} already exists'.format(person))
    else:
	    return personTable.add_person(person)
    

@app.route('/tagapi/addEmail/<conversationID>/<entryID>')
def add_email(conversationID, entryID):
    if emailTable.is_email_in_db(conversationID, entryID):
        return util.insert_rejected('emails entry {0} already exists'.format(entryID))
    else:
	    return emailTable.add_email(conversationID, entryID)
		

@app.route('/tagapi/tagPerson/<person>/<tag>')
def tag_person(person, tag):
    if personTagTable.is_person_tag_in_db(person, tag):
        return util.insert_rejected('person {0} already tagged with {1}'.format(person, tag))
    else:
	    return personTagTable.add_person_tag(person, tag)

@app.route('/tagapi/tagEmail/<entryID>/<tag>')
def tag_email(entryID, tag):
    if emailTagTable.is_email_tag_in_db(entryID, tag):
        return util.insert_rejected('email {0} already tagged with {1}'.format(entryID, tag))
    else:
        return emailTagTable.add_email_tag(entryID, tag)
        

        
@app.route('/tagapi/tagsForEmail/<entryID>')
def get_tags_for_email(entryID):
    return emailTagTable.get_tags_for_email(entryID)


@app.route('/tagapi/docsForTag/<tag>')
def get_docs_for_tag(tag):
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

        
 
    