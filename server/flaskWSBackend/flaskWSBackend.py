#FLASK_APP=flaskWSBackend.py
#FLASK_DEBUG=1

from flask import Flask
from flask import g
import tag
import resource
import email
import person
import resourceTag
import emailTag
import personTag
import tester


DATABASE = 'tagBackend.db'

app = Flask(__name__)

@app.route('/')
def hello_world():
    return 'index page'
 

@app.route('/tagapi/addTag/<tag>')
def add_tag(tag):
	if tag.is_tag_in_db(tag):
	    return insert_rejected('tag {0} already exists'.format(tag))
	else:
	    return tag.add_tag(tag)
		
@app.route('/tagapi/addResource/<type>/<name>')
def add_resource(type, name):
    if resource.is_resource_in_db(type, name):
        return insert_rejected('resource {0} of type {1} already exists'.format(name, type))
    else:
	    return resource.add_resource(type, name)
        
		
@app.route('/tagapi/addPerson/<person>')
def add_person(person):
    if person.is_person_in_db(person):
        return insert_rejected('person {0} already exists'.format(person))
    else:
	    return person.add_person(person)
    

@app.route('/tagapi/addEmail/<conversationID>/<entryID>')
def add_email(conversationID, entryID):
    if email.is_email_in_db(conversationID, entryID):
        return insert_rejected('emails entry {0} already exists'.format(entryID))
    else:
	    return email.add_email(conversationID, entryID)
		

@app.route('/tagapi/tagPerson/<person>/<tag>')
def tag_person(person, tag):
    if (is_person_tag_in_db(person, tag)):
        return insert_rejected('person {0} already tagged with {1}'.format(person, tag))
    else:
	    return personTag.add_person_tag(entryID, conversationID)
        
@app.route('/tagapi/tagsForEmail/<entryID>')
def get_tags_for_email(entryID):
    return emailTag.get_tags_for_email(entryID)


@app.route('/tagapi/docsForTag/<tag>')
def get_docs_for_tag(tag):
    return resourceTag.get_docs_for_tag(tag)
    

@app.route('/tagapi/testing')
def show_user_profile(treename):
    # show the user profile for that user
    return 'flask web services up'

@app.route('/tagapi/dummydocs/<tag>')
def get_dummy_document_tree(tag):
    return resourceTag.get_dummy_document_tree
	
@app.route('/tagapi/runtests')
def run_tests():
    return tester.run_tests()
    
@app.teardown_appcontext
def close_connection(exception):
    db = getattr(g, '_database', None)
    if db is not None:
        db.close()
        
 
    